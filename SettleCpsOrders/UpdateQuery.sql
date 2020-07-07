-- Update ORDER STATUS section --

Select ord_nbr
      ,status 
  from permits.permit_ord 
 where ord_nbr in (1707737855,1707737856,1707739499,1707743931,1707745862,1707746897,1707747746,1707748102,1707755026,1707756853,1707756968,1707756969,1707756971,1707756971,1707756973,1707758157,1708761939,1708769988,1708776154,1708776178,1708776303,1708776374,1708776382,1708776402,1708776455,1708776521,1708776560,1708776624,1708776638,1708776681,1708776727,1708776782,1708776794,1708776799,1708776879,1708776930,1708776964); 

--  Order in COMPLETED status.  Move the orders to BILLED with the scripts below.
--  is in INVOICED status.  No need to update if in Invoiced or Billed status.

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707737855 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707737855; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707737856 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707737856; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707739499 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707739499; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707743931 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707743931; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707745862 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707745862; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707746897 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707746897; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707747746 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707747746; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707748102 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707748102; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707755026 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707755026; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707756853 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707756853; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707756968 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707756968; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707756969 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707756969; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707756971 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707756971; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707756971 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707756971; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707756973 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707756973; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1707758157 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1707758157; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708761939 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708761939; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708769988 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708769988; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776154 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776154; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776178 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776178; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776303 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776303; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776374 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776374; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776382 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776382; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776402 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776402; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776455 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776455; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776521 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776521; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776560 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776560; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776624 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776624; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776638 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776638; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776681 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776681; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776727 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776727; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776782 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776782; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776794 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776794; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776799 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776799; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776879 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776879; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776930 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776930; 
commit; 

update permits.jbpm_token set subprocessinstance_ = null, node_ = 20279295 where id_ = ( 
SELECT 
t.id_ token_id 
FROM permits.jbpm_node n, 
permits.jbpm_token t, 
permits.jbpm_processinstance pi, 
permits.permit_ord ord 				
WHERE ord_nbr = 1708776964 
AND pi.id_ = ord.process_id 
AND t.id_ = pi.roottoken_ 
AND n.id_ = t.node_); 
commit; 
update permits.permit_ord set status = 'BILLED' where ord_nbr = 1708776964; 
commit; 

-- Update BILLING record status section so records will go to Lawson invoice file when NGC invoice job runs --

Select ord_nbr
      ,status 
  from permits.billing 
 where ord_nbr in (1707737855,1707737856,1707739499,1707743931,1707745862,1707746897,1707747746,1707748102,1707755026,1707756853,1707756968,1707756969,1707756971,1707756971,1707756973,1707758157,1708761939,1708769988,1708776154,1708776178,1708776303,1708776374,1708776382,1708776402,1708776455,1708776521,1708776560,1708776624,1708776638,1708776681,1708776727,1708776782,1708776794,1708776799,1708776879,1708776930,1708776964) 
   and app_code = 'PERMITS'; 

-- Update any records for these orders to OPEN status in Billing table that aren't in CLOSED status.

Update permits.billing 
   set status = 'OPEN' 
 where ord_nbr in (1707737855,1707737856,1707739499,1707743931,1707745862,1707746897,1707747746,1707748102,1707755026,1707756853,1707756968,1707756969,1707756971,1707756971,1707756973,1707758157,1708761939,1708769988,1708776154,1708776178,1708776303,1708776374,1708776382,1708776402,1708776455,1708776521,1708776560,1708776624,1708776638,1708776681,1708776727,1708776782,1708776794,1708776799,1708776879,1708776930,1708776964) 
   and app_code = 'PERMITS'
   and status <> 'CLOSED';  
commit; 

-- Section to generate credit card receipts. -- 

select ord_nbr
      ,status 
  from permits.cr_crd_pmt_dtl 
 where ord_nbr in (1707737855,1707737856,1707739499,1707743931,1707745862,1707746897,1707747746,1707748102,1707755026,1707756853,1707756968,1707756969,1707756971,1707756971,1707756973,1707758157,1708761939,1708769988,1708776154,1708776178,1708776303,1708776374,1708776382,1708776402,1708776455,1708776521,1708776560,1708776624,1708776638,1708776681,1708776727,1708776782,1708776794,1708776799,1708776879,1708776930,1708776964);

-- All orders show either REAUTH or WAIT status, so update them to READY to genereate

Update permits.cr_crd_pmt_dtl 
   set status = 'READY' 
 where ord_nbr in (1707737855,1707737856,1707739499,1707743931,1707745862,1707746897,1707747746,1707748102,1707755026,1707756853,1707756968,1707756969,1707756971,1707756971,1707756973,1707758157,1708761939,1708769988,1708776154,1708776178,1708776303,1708776374,1708776382,1708776402,1708776455,1708776521,1708776560,1708776624,1708776638,1708776681,1708776727,1708776782,1708776794,1708776799,1708776879,1708776930,1708776964);    
commit;

-- NOTE!!! Please call Scott Langley at x6846 if you have not already done so.
-- *** Before proceeding to next update, the NGC credit card receipts job must be run on BWRCSAPP.  It is E:\Batch\ngp\bin\CreditCardReceiptsJob.bat.
-- It can also be run from the mainframe by running only DIPCD001 (no triggers to start other jobs). ***

-- This update will mark the orders as SETTLED.  They will no longer appear on the Settlement Failure report after this update.

Update permits.cr_crd_pmt_dtl 
   set status = 'SETTLED' 
 where ord_nbr in (1707737855,1707737856,1707739499,1707743931,1707745862,1707746897,1707747746,1707748102,1707755026,1707756853,1707756968,1707756969,1707756971,1707756971,1707756973,1707758157,1708761939,1708769988,1708776154,1708776178,1708776303,1708776374,1708776382,1708776402,1708776455,1708776521,1708776560,1708776624,1708776638,1708776681,1708776727,1708776782,1708776794,1708776799,1708776879,1708776930,1708776964); 
commit; 