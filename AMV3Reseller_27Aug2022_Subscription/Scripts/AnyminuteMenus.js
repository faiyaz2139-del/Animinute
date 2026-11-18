/* v88: position kebab / action menus as fixed overlays so they are not clipped by cards or table scrollers. */
(function () {
    var SEL = "details.am-sd-menu,details.am-menu,details.am-employee-menu,details.am-business-status-menu,details.am-ed-menu,details.am-pj-menu,details.sa-menu,details.am-business-menu";
    function boxOf(d) {
        var kids = d.children, i, el, tag;
        for (i = 0; i < kids.length; i++) {
            el = kids[i];
            tag = (el.tagName || "").toUpperCase();
            if (tag === "DIV" || tag === "UL" || (el.className && /am-row-menu|am-pj-pop/.test(el.className))) return el;
        }
        return null;
    }
    function place(d) {
        var box = boxOf(d), sum, r, w, h, left, top;
        if (!box) return;
        if (!d.open) {
            box.style.position = "";
            box.style.left = "";
            box.style.top = "";
            box.style.right = "";
            box.style.bottom = "";
            box.style.zIndex = "";
            box.style.maxHeight = "";
            box.style.overflowY = "";
            return;
        }
        sum = d.querySelector("summary") || d;
        r = sum.getBoundingClientRect();
        box.style.position = "fixed";
        box.style.right = "auto";
        box.style.bottom = "auto";
        box.style.zIndex = "4000";
        box.style.visibility = "hidden";
        box.style.left = "0px";
        box.style.top = "0px";
        w = box.offsetWidth || 180;
        h = box.offsetHeight || 0;
        left = r.right - w;
        if (left < 8) left = 8;
        if (left + w > window.innerWidth - 8) left = Math.max(8, window.innerWidth - w - 8);
        top = r.bottom + 4;
        if (top + h > window.innerHeight - 8) {
            top = r.top - h - 4;
            if (top < 8) {
                top = 8;
                box.style.maxHeight = (window.innerHeight - 16) + "px";
                box.style.overflowY = "auto";
            }
        }
        box.style.left = left + "px";
        box.style.top = top + "px";
        box.style.visibility = "";
    }
    function placeOpen() {
        var nodes = document.querySelectorAll(SEL), i;
        for (i = 0; i < nodes.length; i++) if (nodes[i].open) place(nodes[i]);
    }
    document.addEventListener("toggle", function (e) {
        var d = e.target;
        if (!d || d.tagName !== "DETAILS") return;
        if (!d.matches || !d.matches(SEL)) return;
        place(d);
    }, true);
    window.addEventListener("scroll", placeOpen, true);
    window.addEventListener("resize", placeOpen);
})();
