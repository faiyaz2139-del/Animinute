import re,sys
src='/workspace/anymin/src/AMV3Reseller_27Aug2022_Subscription/'
BOOL={'visible','enabled','checked','selected','readonly','causesvalidation','autopostback','enableviewstate'}
INT={'maxlength','rows','columns','tabindex'}
U="using System; using System.Collections; using System.Linq; using System.Web; using System.Web.UI; using System.Web.UI.WebControls; using System.Web.UI.HtmlControls;"
for f in sys.argv[1:]:
    a=open(src+f,encoding='utf-8-sig').read()
    cls=re.search(r'Inherits="([^"]+)"',a).group(1)
    a=re.sub(r'<%--.*?--%>','',a,flags=re.S)
    lines=[];props=set()
    for m in re.finditer(r'<(asp:\w+|\w+)\b((?:[^>"\']|"[^"]*"|\'[^\']*\')*?)/?>',a,flags=re.S):
        for pm in re.finditer(r'([\w-]+)\s*=\s*([\'"])<%#:?(.*?)%>\2',m.group(2),flags=re.S):
            pn,expr=pm.group(1).lower(),pm.group(3).strip()
            props.add(pn)
            if pn in BOOL: lines.append('        { bool __b = ((bool)(%s)); }'%expr)
            elif pn in INT: lines.append('        { int __i = ((int)(%s)); }'%expr)
    name='Cast_'+re.sub(r'\W','_',f)
    open('/tmp/cc86b/'+re.sub(r'\W','_',f)+'.cast.cs','w').write(U+'\npublic class %s : %s {\n    void __Check(global::System.Web.UI.WebControls.RepeaterItem Container) {\n%s\n    }\n}\n'%(name,cls,'\n'.join(lines)))
    print(f,len(lines),sorted(props))
