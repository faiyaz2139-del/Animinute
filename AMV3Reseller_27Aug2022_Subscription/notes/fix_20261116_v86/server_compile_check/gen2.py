import re,sys
src='/workspace/anymin/src/AMV3Reseller_27Aug2022_Subscription/'
HT={'a':'HtmlAnchor','form':'HtmlForm','img':'HtmlImage','table':'HtmlTable','tr':'HtmlTableRow','td':'HtmlTableCell','th':'HtmlTableCell','select':'HtmlSelect','head':'HtmlHead','title':'HtmlTitle','link':'HtmlLink','meta':'HtmlMeta','textarea':'HtmlTextArea','button':'HtmlButton','iframe':'HtmlIframe'}
IT={'text':'HtmlInputText','password':'HtmlInputPassword','button':'HtmlInputButton','submit':'HtmlInputSubmit','reset':'HtmlInputReset','checkbox':'HtmlInputCheckBox','radio':'HtmlInputRadioButton','hidden':'HtmlInputHidden','file':'HtmlInputFile','image':'HtmlInputImage'}
CUSTOM={'am:UiOverview':'AnyminuteUiOverview'}
TPL=r'<(ItemTemplate|AlternatingItemTemplate|HeaderTemplate|FooterTemplate|EditItemTemplate|EmptyDataTemplate|SeparatorTemplate|LayoutTemplate|Columns|InsertItemTemplate|SelectedItemTemplate|EmptyItemTemplate|GroupTemplate|PagerTemplate)\b[^>]*>.*?</\1>'
for f in sys.argv[1:]:
    a=open(src+f,encoding='utf-8-sig').read()
    d=re.search(r'<%@\s*(Page|Master|Control)\b([^%]*)%>',a)
    cls=re.search(r'Inherits="([^"]+)"',d.group(2)).group(1)
    a=re.sub(r'<%--.*?--%>','',a,flags=re.S); a=re.sub(TPL,'',a,flags=re.S)
    seen=set();out=['public partial class %s {'%cls]
    for m in re.finditer(r'<([\w]+(?::\w+)?)\b((?:[^>"]|"[^"]*")*?)/?>',a,flags=re.S):
        tag,attrs=m.group(1),m.group(2)
        im=re.search(r'\bid\s*=\s*"(\w+)"',attrs,re.I)
        if not im: continue
        i=im.group(1)
        if tag.startswith('asp:'): t=('global::System.Web.UI.' if tag[4:] in ('ScriptManager','ScriptManagerProxy','UpdatePanel','UpdateProgress','Timer') else 'global::System.Web.UI.WebControls.')+tag[4:]
        elif tag in CUSTOM: t='global::'+CUSTOM[tag]
        elif re.search(r'runat\s*=\s*"?server',attrs,re.I):
            tl=tag.lower()
            if tl=='input':
                ty=re.search(r'\btype\s*=\s*"(\w+)"',attrs,re.I); t='global::System.Web.UI.HtmlControls.'+IT.get(ty.group(1).lower() if ty else 'text','HtmlInputGenericControl')
            else: t='global::System.Web.UI.HtmlControls.'+HT.get(tl,'HtmlGenericControl')
        else: continue
        if i in seen: print('DUP',f,i); continue
        seen.add(i); out.append('protected %s %s;'%(t,i))
    # members the ASP.NET page parser always adds to the CodeFile partial class
    out.append('protected global::System.Web.Profile.DefaultProfile Profile { get { return null; } }')
    out.append('protected global::System.Web.HttpApplication ApplicationInstance { get { return null; } }')
    out.append('}')
    open('/tmp/cc86b/'+f.replace('/','_')+'.designer.cs','w').write('\n'.join(out)+'\n')
    print(f,cls,len(seen))
