import re,os,glob,collections
R='/workspace/anymin/src/AMV3Reseller_27Aug2022_Subscription/'
pages=['Add_Business.aspx','BusinessProfile.aspx','Dashboard.aspx','EmployeeAssignment.aspx','EmployeeDetails.aspx','EmployeeEditor.aspx','EmployeeProfile.aspx','WeekTimeCard.aspx','Main.master','MasterWithoutMenu.master','Modern.master']
GEN={'Profile','ApplicationInstance','FrameworkInitialize','GetTypeHashCode','ProcessRequest','__initialized','__fileDependencies','__stringResource'}
BASE={'Title','Request','Response','Session','Cache','Context','Master','Form','Header','Controls','ID','Page','Server','User','Trace','Items','Application','Validators','ClientScript','ViewState','Site','Parent','Visible','Events','Culture','Theme','MasterPageFile','EnableViewState','ClientID','UniqueID','NamingContainer','ContentPlaceHolders','IsPostBack','Init','Load','PreRender','Unload','Error','DataBind','Dispose','Focus','Validate','Async','StyleSheetTheme','Adapter','Content','Body','Head'}
TPL=r'<(ItemTemplate|AlternatingItemTemplate|HeaderTemplate|FooterTemplate|EditItemTemplate|EmptyDataTemplate|SeparatorTemplate|LayoutTemplate|ContentTemplate|Columns|ProgressTemplate|InsertItemTemplate|SelectedItemTemplate|EmptyItemTemplate|GroupTemplate|PagerTemplate)\b[^>]*>(.*?)</\1>'
def ids(txt):
    out=[]
    for m in re.finditer(r'<(asp:\w+|\w+)\b([^>]*?)>',txt,flags=re.S):
        tag,attrs=m.group(1),m.group(2)
        if not tag.lower().startswith('asp:') and not re.search(r'runat\s*=\s*"?server',attrs,re.I): continue
        im=re.search(r'\bid\s*=\s*"([^"]+)"',attrs,re.I)
        if im: out.append((im.group(1),tag))
    return out
allclasses=collections.defaultdict(list)
for f in glob.glob(R+'**/*.cs',recursive=True):
    if '/notes/' in f or '/LogBackup/' in f or '/packages/' in f or '/.vs/' in f or '/obj/' in f: continue
    s=open(f,encoding='utf-8-sig',errors='replace').read()
    s=re.sub(r'//[^\n]*|/\*.*?\*/','',s,flags=re.S)
    for m in re.finditer(r'\b(?:class|struct|interface|enum)\s+(\w+)',s):
        if re.search(r'\bpartial\s+class\s+'+m.group(1)+r'\b',s): allclasses[m.group(1)].append(f.replace(R,'')+'(partial)')
        else: allclasses[m.group(1)].append(f.replace(R,''))
for p in pages:
    a=open(R+p,encoding='utf-8-sig').read()
    a_nc=re.sub(r'<%--.*?--%>','',a,flags=re.S)
    d=re.search(r'<%@\s*(Page|Master|Control)\b([^%]*)%>',a_nc)
    inh=re.search(r'Inherits="([^"]+)"',d.group(2)); cf=re.search(r'CodeFile="([^"]+)"',d.group(2))
    print('==',p,'Inherits=',inh and inh.group(1),'CodeFile=',cf and cf.group(1))
    cls=inh.group(1) if inh else None
    cs=''
    if cf:
        cs=open(R+cf.group(1),encoding='utf-8-sig').read()
        if not re.search(r'partial\s+class\s+'+re.escape(cls)+r'\b',cs): print('  !! Inherits class not partial-declared in CodeFile')
    # templates
    templ=[m.group(2) for m in re.finditer(TPL,a_nc,flags=re.S)]
    top=re.sub(TPL,'',a_nc,flags=re.S)
    tops=ids(top)
    c=collections.Counter(i for i,_ in tops)
    for i,n in c.items():
        if n>1: print('  !! duplicate top-level ID',i,n)
    for t in templ:
        tc=collections.Counter(i for i,_ in ids(t))
        for i,n in tc.items():
            if n>1: print('  !! duplicate ID inside one template',i,n)
    csn=re.sub(r'//[^\n]*|/\*.*?\*/|"(?:\\.|[^"\\])*"','',cs,flags=re.S)
    members=set(re.findall(r'(?:void|string|int|bool|object|var|DataTable|[A-Z]\w*(?:<[^>]*>)?(?:\[\])?)\s+(\w+)\s*(?:\(|\{|=|;)',csn))
    for i,tag in tops:
        if i in GEN: print('  !! ID clashes with generated member:',i,tag)
        if i in BASE: print('  ?  ID hides base member:',i,tag)
        if cls and i==cls.split('.')[-1]: print('  !! ID equals class name',i)
        # declared in code-behind as field/property/method?
        if re.search(r'\b(?:protected|public|private|internal)?\s*(?:static\s+)?[\w.<>\[\],]+\s+'+re.escape(i)+r'\s*(\(|\{|;|=)',csn) and not re.search(r'new\s+\w+\s+'+i,csn):
            # exclude local usage like "Literal x = Profile"
            for m in re.finditer(r'(?m)^\s*(?:protected|public|private|internal)[^\n;{(]*\b'+re.escape(i)+r'\s*(\(|\{|;|=)',csn):
                print('  !! code-behind declares member with same name as control ID:',i,'->',m.group(0).strip()[:90])
    for g in GEN:
        if re.search(r'(?m)^\s*(?:protected|public|private|internal)[^\n;{(]*\b'+g+r'\s*(\(|\{|;|=)',csn): print('  !! code-behind declares generated-member name',g)
dup={k:v for k,v in allclasses.items() if len([x for x in v if not x.endswith('(partial)')])>1 or (len(set(x.replace('(partial)','') for x in v))>1 and any(x.endswith('(partial)') for x in v))}
newcls=['AnyminuteBusinessProfilePage','AnyminuteSubscriberDashboard','AnyminuteEmployeeDocuments','AnyminuteEmployeeDetails','AnyminuteEmployeeAssignmentPage','AnyminuteDocItem','AnyminuteEmployeeEditor']
for k,v in sorted(dup.items()):
    print('class',k,'declared in',len(v),'files:',v[:6], '<-- NEW/CHANGED' if any(n in k for n in newcls) else '')
