-- Update ORDER STATUS section --

Select ord_nbr
      ,status 
  from permits.permit_ord 
 where ord_nbr in ({0}); 

--  Order in COMPLETED status.  Move the orders to BILLED with the scripts below.
--  is in INVOICED status.  No need to update if in Invoiced or Billed status.

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = {0} 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = {0}; 
commit; 

-- Update BILLING record status section so records will go to Lawson invoice file when NGC invoice job runs --

Select ord_nbr
      ,status 
  from permits.billing 
 where ord_nbr in ({0}) 
   and app_code = 'PERMITS'; 

-- Update any records for these orders to OPEN status in Billing table that aren't in CLOSED status.

Update permits.billing 
   set status = 'OPEN' 
 where ord_nbr in ({0}) 
   and app_code = 'PERMITS'
   and status <> 'CLOSED';  
commit; 

-- Section to generate credit card receipts. -- 

select ord_nbr
      ,status 
  from permits.cr_crd_pmt_dtl 
 where ord_nbr in ({0});

-- All orders show either REAUTH or WAIT status, so update them to READY to genereate

Update permits.cr_crd_pmt_dtl 
   set status = 'READY' 
 where ord_nbr in ({0});    
commit;

-- NOTE!!! Please call Scott Langley at x6846 if you have not already done so.
-- *** Before proceeding to next update, the NGC credit card receipts job must be run on BWRCSAPP.  It is E:\Batch\ngp\bin\CreditCardReceiptsJob.bat.
-- It can also be run from the mainframe by running only DIPCD001 (no triggers to start other jobs). ***

-- This update will mark the orders as SETTLED.  They will no longer appear on the Settlement Failure report after this update.

Update permits.cr_crd_pmt_dtl 
   set status = 'SETTLED' 
 where ord_nbr in ({0}); 
commit; 