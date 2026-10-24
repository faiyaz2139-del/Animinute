$(function () {
    // Select Upload-Area
    var uploadArea = document.querySelector('#uploadArea');
    var dropZoon = document.querySelector('#dropZoon');
    var loadingText = document.querySelector('#loadingText');
    var fileInput = document.querySelector('#<%=FileUpload1.ClientID %>');
    var previewImage = document.querySelector('#previewImage');
    var imagesTypes = [
        "jpeg",
        "png",
        "svg",
        "gif"
    ];

    // When (drop-zoon) has (dragover) Event 
    dropZoon.addEventListener('dragover', function (event) {
        // Prevent Default Behavior 
        event.preventDefault();
        dropZoon.classList.add('drop-zoon--over');
    });

    // When (drop-zoon) has (dragleave) Event 
    dropZoon.addEventListener('dragleave', function (event) {
        dropZoon.classList.remove('drop-zoon--over');
    });

    // When (drop-zoon) has (drop) Event 
    dropZoon.addEventListener('drop', function (event) {
        event.preventDefault();
        dropZoon.classList.remove('drop-zoon--over');
        const file = event.dataTransfer.files[0];
        uploadFile(file);
    });

    // When (drop-zoon) has (click) Event 
    dropZoon.addEventListener('click', function (event) {
        fileInput.click();
    });

    // When (fileInput) has (change) Event 
    fileInput.addEventListener('change', function (event) {
        var file = event.target.files[0];
        uploadFile(file);
    });

    // Upload File Function
    function uploadFile(file) {
        var fileReader = new FileReader();
        var fileType = file.type;
        var fileSize = file.size;

        // If File Is Passed from the (File Validation) Function
        if (fileValidate(fileType, fileSize)) {
            dropZoon.classList.add('drop-zoon--Uploaded');
            loadingText.style.display = "block";
            previewImage.style.display = 'none';
            $('.drop-zoon__paragraph').show();
            // After File Reader Loaded 
            fileReader.addEventListener('load', function () {
                setTimeout(function () {
                    uploadArea.classList.add('upload-area--open');
                    loadingText.style.display = "none";
                    previewImage.style.display = 'block';

                }, 500); // 0.5s

                // Add The (fileReader) Result Inside (previewImage) Source
                previewImage.setAttribute('src', fileReader.result);
            });

            // Read (file) As Data Url 
            fileReader.readAsDataURL(file);
        } else {
            this; // (this) Represent The fileValidate(fileType, fileSize) Function
        };
    };

    // Simple File Validate Function
    function fileValidate(fileType, fileSize) {
        // File Type Validation
        let isImage = imagesTypes.filter(function (type) {
            return fileType.indexOf(`image/${type}`) !== -1
        });
        // If The Uploaded File Is An Image
        if (isImage.length !== 0) {
            // Check, If File Size Is 2MB or Less
            if (fileSize <= 2000000) { // 2MB :)
                return true;
            } else { // Else File Size
                return alert('Please Your File Should be 2 Megabytes or Less');
            };
        } else { // Else File Type 
            return alert('Please make sure to upload An Image File Type');
        };
    };
});