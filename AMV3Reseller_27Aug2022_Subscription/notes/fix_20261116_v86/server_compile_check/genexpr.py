import re,sys
src='/workspace/anymin/src/AMV3Reseller_27Aug2022_Subscription/'
n=0
for f in sys.argv[1:]:
    a=open(src+f,encoding='utf-8-sig').read()
    d=re.search(r'<%@\s*(Page|Master|Control)\b([^%]*)%>',a)
    cls=re.search(r'Inherits="([^"]+)"',d.group(2)).group(1)
    a=re.sub(r'<%--.*?--%>','',a,flags=re.S)
    lines=[]
    for m in re.finditer(r'<%(?!@|--)(#:|#|=|:|\$)?(.*?)%>',a,flags=re.S):
        kind,expr=m.group(1) or '',m.group(2).strip()
        if kind=='$': continue
        if kind in ('#','#:','=',':'):
            lines.append('        { object __v = (%s); }'%expr)
        else:
            lines.append('        { %s }'%expr)
        n+=1
    name='Gen_'+re.sub(r'\W','_',f)
    out=['using System; using System.Collections; using System.Collections.Specialized; using System.Configuration; using System.Text; using System.Text.RegularExpressions; using System.Linq; using System.Web; using System.Web.Caching; using System.Web.SessionState; using System.Web.Security; using System.Web.Profile; using System.Web.UI; using System.Web.UI.WebControls; using System.Web.UI.WebControls.WebParts; using System.Web.UI.HtmlControls;','public class %s : %s {'%(name,cls),'    void __Check(global::System.Web.UI.WebControls.RepeaterItem Container) {']+lines+['    }','}']
    open('/tmp/cc86b/'+re.sub(r'\W','_',f)+'.expr.cs','w').write('\n'.join(out)+'\n')
    print(f,len(lines))
