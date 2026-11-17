import re
R='/workspace/anymin/src/AMV3Reseller_27Aug2022_Subscription/'
pages=['Add_Business.aspx','BusinessProfile.aspx','Dashboard.aspx','EmployeeAssignment.aspx','EmployeeDetails.aspx','EmployeeEditor.aspx','EmployeeProfile.aspx','WeekTimeCard.aspx','Main.master','MasterWithoutMenu.master','Modern.master']
ALLOW={'Literal':{'id','runat','text','mode','visible','enableviewstate','viewstatemode','clientidmode'},
 'PlaceHolder':{'id','runat','visible','enableviewstate','viewstatemode','clientidmode'},
 'Repeater':{'id','runat','datasource','datasourceid','datamember','onitemcommand','onitemdatabound','onitemcreated','visible','enableviewstate','viewstatemode','clientidmode'},
 'HiddenField':{'id','runat','value','clientidmode','visible','onvaluechanged','enableviewstate','viewstatemode'},
 'Content':{'id','runat','contentplaceholderid'},
 'ListItem':{'value','text','selected','enabled'},
 'ContentPlaceHolder':{'id','runat'}}
for p in pages:
    a=open(R+p,encoding='utf-8-sig').read(); a=re.sub(r'<%--.*?--%>','',a,flags=re.S)
    b=re.sub(r'<%.*?%>',lambda m:'X'*len(m.group(0)) if '"' not in m.group(0) else re.sub(r'[^\n]','X',m.group(0)),a,flags=re.S)
    for m in re.finditer(r'<asp:(\w+)\b((?:[^>"\']|"[^"]*"|\'[^\']*\')*?)/?>',b,flags=re.S):
        tag,attrs=m.group(1),m.group(2)
        names=[x.lower() for x in re.findall(r'([\w:-]+)\s*=',attrs)]
        if tag!='ListItem' and 'runat' not in names: print(p,'missing runat',tag,attrs[:60])
        if tag in ALLOW:
            bad=[n for n in names if n not in ALLOW[tag]]
            if bad: print(p,'unknown attr on',tag,bad)
    for m in re.finditer(r'<asp:\w+[^>]*=\s*"<%[=:]',a): print(p,'<%= inside server-control attribute:',m.group(0)[:80])
    # ids duplicated anywhere (incl templates) for info
print('done')
