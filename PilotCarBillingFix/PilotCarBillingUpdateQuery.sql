-- Have DBA run these update scripts for each order that appears in the query above.
-- There are three update scripts.  The first one will always be run.  Only one of the 2nd and 3rd update script will be run,
-- depending on whether they execute it before or after the Pilot Car Billing job has run for the day. (it runs at 2:35 p.m.)

-- To validate, input your order numbers in the query below.  
-- If, after 3:10 or 3:15 p.m. (invoice job runs at 3:00 p.m.) of the day we show for the billing movement date, any orders are not in INVOICED status,
-- another ITASK will need to be created to move the order to INVOICED.  
select ord_nbr
      ,ord_status
      ,blng_mvmnt_dt
   from permits.pc_ord
  where ord_nbr in ();
