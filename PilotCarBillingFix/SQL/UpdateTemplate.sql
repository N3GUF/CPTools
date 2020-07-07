-- Have DBA run these update scripts for each order that appears in the query above.
-- There are three update scripts.  The first one will always be run.  Only one of the 2nd and 3rd update script will be run,
-- depending on whether they execute it before or after the Pilot Car Billing job has run for the day. (it runs at 2:35 p.m.)

-- Completed [Billing] -  (try this with 20280262 for the node_ sometime)
update permits.jbpm_token set subprocessinstance_ = null, node_ = 20280253 where id_ = (
SELECT 
  t.id_ token_id
FROM permits.jbpm_node n,
  permits.jbpm_token t,
  PERMITS.jbpm_processinstance pi,
  PERMITS.pc_ord ord
WHERE ord_nbr = {0}          
AND pi.id_    = ord.process_id
AND t.id_     = pi.roottoken_
AND n.id_     = t.node_);
commit;

-- **** NOTE **** If running this before 2:35 p.m., run the script with sysdate. If after 2:35 p.m., run the script with sysdate + 1 (for next day)
-- For Pilot Car, set billing movement date to same day if before pcbillingjob run (2:35p).  
update permits.pc_ord set ord_status = 'COMPLETED', blng_mvmnt_dt = sysdate where ord_nbr = {0};
commit;
-- Set it to blng_mvmnt_dt + 1 if after pcbillingjob run (2:35p)     (or sysdate + 3 if today is Friday and it is after 2:35 p.m. so it will bill Monday)
update permits.pc_ord set ord_status = 'COMPLETED', blng_mvmnt_dt = sysdate + 1 where ord_nbr = {0};
commit;

-- To validate, input your order numbers in the query below.  
-- If, after 3:10 or 3:15 p.m. (invoice job runs at 3:00 p.m.) of the day we show for the billing movement date, any orders are not in INVOICED status,
-- another ITASK will need to be created to move the order to INVOICED.  
select ord_nbr
      ,ord_status
      ,blng_mvmnt_dt
   from permits.pc_ord
  where ord_nbr in ({0});
