;#DropDB GAVELISv2EasyTest

#Application GAVELISv2.Win

*Action Navigation(User)

*ProcessRecord
 User Name = [DomainUserName]
 
*FillForm
 User Name = Test User
 
*Action Cancel

*HandleDialog
 Respond = Yes

*CheckFieldValues
 User Name = [DomainUserName]

