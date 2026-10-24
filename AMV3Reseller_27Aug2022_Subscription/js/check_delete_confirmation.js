// Run: node tests/check_delete_confirmation.js. No browser or database is contacted.
const fs = require('fs'), vm = require('vm'), assert = require('assert'), path = require('path');
let live = [], confirms = 0, posts = 0;
class Element {
  constructor(tag) { this.tag=tag; this.children=[]; this.events={}; this.dataset={}; this.style={}; }
  setAttribute() {} appendChild(x) {this.children.push(x);}
  addEventListener(n,f) {this.events[n]=f;} focus() {} showModal() {}
  close() {if(this.events.close) this.events.close();}
  remove() {live=live.filter(x=>x!==this);}
}
const document={createElement:t=>new Element(t), getElementById:id=>live.find(e=>e.id===id), body:{appendChild:e=>live.push(e)}};
const window={confirm:()=>{confirms++;return false;}};
vm.runInNewContext(fs.readFileSync(path.join(__dirname,'../../site/Scripts/AnyminuteConfirm.js'),'utf8'),{window,document});
const button=new Element('button'); button.click=()=>{if(window.AnyminuteConfirmDelete(button))posts++;};
assert.strictEqual(window.AnyminuteConfirmDelete(button),false);
assert.strictEqual(posts,0);
let dialog=live[0]; assert.strictEqual(dialog.children[2].textContent,'No');
dialog.children[2].events.click(); assert.strictEqual(posts,0); assert.strictEqual(live.length,0);
window.AnyminuteConfirmDelete(button);dialog=live[0];dialog.children[3].events.click();
assert.strictEqual(posts,1);assert.strictEqual(button.dataset.deleteConfirmed,undefined);
assert.strictEqual(window.AnyminuteConfirmDelete(button),false); // Approval is one-use only.
live[0].close(); assert.strictEqual(posts,1);
document.createElement=t=>{let e=new Element(t);e.showModal=undefined;return e;};
assert.strictEqual(window.AnyminuteConfirmDelete(button),false);assert.strictEqual(confirms,1);
console.log('PASS: No/close never posts; Yes posts once; approval is one-use; fallback cancel never posts.');
