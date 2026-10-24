const fs=require('fs'),vm=require('vm'),assert=require('assert');
const source=fs.readFileSync(__dirname+'/../../site/Scripts/AnyminuteTables.js','utf8');
function table(width,bodyWidths){const row=n=>({cells:Array.from({length:n},()=>({colSpan:1,rowSpan:1}))});return {tHead:{rows:[row(width)]},tBodies:[{rows:bodyWidths.map(row)}]};}
function run(t,initialized=false){let calls=0;const handlers=[];const $=arg=>{if(typeof arg==='function'){arg();return;}return {DataTable(){calls++;initialized=true;}}};$.fn={dataTable:{isDataTable:()=>initialized}};vm.runInNewContext(source,{window:{jQuery:$,document:{getElementById:id=>id==='GvEmpListForBusOwner'?t:null},Sys:{Application:{add_load:fn=>handlers.push(fn)}}}});handlers.forEach(h=>h());return calls;}
assert.equal(run(table(6,[6,6])),1);
assert.equal(run(table(6,[6]),true),0);
assert.equal(run(null),0);
let t=table(6,[6]);t.tHead=null;assert.equal(run(t),0);
assert.equal(run(table(6,[])),0);
assert.equal(run(table(6,[5])),0);
t=table(6,[1]);t.tBodies[0].rows[0].cells[0].colSpan=6;assert.equal(run(t),0);
t=table(6,[6]);t.tBodies[0].rows[0].cells[0].rowSpan=2;assert.equal(run(t),0);
console.log('8 table enhancement checks PASS');
