select distinct po.ord_nbr
      from permits.pc_ord po
inner join permits.pc_leg pl 
        on pl.ord_nbr = po.ord_nbr
     where po.ord_nbr > 1601000000
       and po.ord_status in ('BILLED','INVOICED')
       and pl.leg_status = 'PAID'
       and po.ord_nbr not in (select ord_nbr 
	                            from permits.billing 
							   where ord_nbr > 1601000000 
							     and app_code = 'PILOTCAR')
   order by po.ord_nbr