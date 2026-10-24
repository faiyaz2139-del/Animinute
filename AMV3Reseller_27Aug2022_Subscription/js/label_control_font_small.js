$(document).ready(function () {
    if (screen.width < 1600) {

        function changeFont(element) {
            element.setAttribute("style", element.getAttribute("style") + ";");
            for (var i = 0; i < element.children.length; i++) {
                changeFont(element.children[i]);
            }
        }

        changeFont(document.getElementsByTagName("body")[0]);

        //function mapElement(element, callback) {
        //    callback(element);
        //    for (var i = 0; i < element.children.length; i++) {
        //        callback(element.children[i]);
        //    }
        //}

        //mapElement(
        //    document.querySelector('body'),
        //    (e) => {
        //        e.style.fontFamily = 'Arial';
        //        e.style.fontSize = '10pt';
        //    }
        //);


        var originalSize = $('div').css('font-size');
        // reset        
        // $(".resetMe").click(function(){           
        //$('div').css('font-size', originalSize);         
        // });

        // Increase Font Size          
        // $(".increase").click(function(){         
        //var currentSize = $('div').css('font-size');         
        //var currentSize = parseFloat(currentSize)*1.2;          
        //$('div').css('font-size', currentSize);         
        //return false;          
        //});        

        // Decrease Font Size       
        //$(".decrease").click(function(){        
        var currentFontSize = $('div').css('font-size');
        var currentSize = $('div').css('font-size');
        var currentSize = parseFloat(currentSize) * 0.8;
        $('div').css('font-size', currentSize);
        return false;
        //}); 




        var originalSize1 = $('button').css('font-size');
        var currentFontSize1 = $('button').css('font-size');
        var currentSize1 = $('button').css('font-size');
        var currentSize1 = parseFloat(currentSize1) * 0.5;
        $('button').css('font-size', currentSize1);
        return false;
    }
});



//$(document).ready(function () {
//    if (screen.width < 1600) {
//        var originalSize2 = $("input").css('font-size');
//        var currentFontSize2 = $("input").css('font-size');
//        var currentSize2 = $("input").css('font-size');
//        var currentSize2 = parseFloat(currentSize2) * 0.8;
//        $("input").css('font-size', currentSize2);
//        return false;
//    }
//});