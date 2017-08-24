// Hides any active overlay elements.
function closeWindowClick() {
    $('.overlay').addClass('hidden');
}

// Hides an overlay element when the user clicks outside the innver div of the overlay.
function hideOverlayDiv(overlay) {
    if (event.target === overlay) {
        overlay.classList.add('hidden');
    }
}

function getSelectedValue(cbo) {
    var value = '';
    if (cbo) {
        value = cbo.options[cbo.selectedIndex].value;
    }
    return value;
}

function inquiryChange(cboInquiry, tbTarget) {
    var cboValue = getSelectedValue(cboInquiry);
    var tb = document.getElementById(tbTarget);

    if (tb) {
        tb.value = '';
        if (cboValue === 'S') {
            tb.maxLength = 8;
        } else {
            tb.maxLength = 10;
        }
    }
}

jQuery.fn.forceNumeric = function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.which || e.keyCode;

            if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
                // numbers   
                key >= 48 && key <= 57 ||
                // Numeric keypad
                key >= 96 && key <= 105 ||
                // comma, period and minus, . on keypad
               key == 190 || key == 188 || key == 109 || key == 110 ||
                // Backspace and Tab and Enter
               key == 8 || key == 9 || key == 13 ||
                // Home and End
               key == 35 || key == 36 ||
                // left and right arrows
               key == 37 || key == 39 ||
                // Del and Ins
               key == 46 || key == 45)
                return true;

            return false;
        });
    });
}