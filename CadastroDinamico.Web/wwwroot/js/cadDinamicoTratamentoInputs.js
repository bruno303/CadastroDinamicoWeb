function setMaskDateTime(mask) {
    $(mask).inputmask("datetime", {
        mask: "1/2/y h:s",
        placeholder: "dd/mm/yyyy hh:mm",
        alias: "datetime",
        separator: "/",
        hourFormat: "24"
    });
}

function tratarKeyDown(event) {
    const allowed = [190, 37, 38, 39, 40, 8, 9, 46, 27, 35, 36, 13];

    if (allowed.filter(num => num === event.keyCode).length > 0) {
        if (event.keyCode === 190) { // Ponto "."
            if (event.target.value.toString().indexOf(".") > -1) {
                event.preventDefault();
            }
        }
    } else if (event.keyCode < 48 || event.keyCode > 57) {
        event.preventDefault();
    }
}