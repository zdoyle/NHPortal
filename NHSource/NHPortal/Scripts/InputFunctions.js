function replaceAll(txt, findVal, replaceVal) {
    return txt.split(findVal).join(replaceVal);
}

function DenyLetters(e) {
    var letter;
    var allDeniedChars, letterIndex;

    try {
        allDeniedChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
        letter = GetLetterFromEvent(e);
        if (letter != '') {
            letterIndex = allDeniedChars.indexOf(letter.toUpperCase());
            return letterIndex < 0;
        }
        else {
            return true;
        }
    }
    catch (genEx) {
        return true;
    }
}

function DenyNonDigits(e) {
    if (IsSpecialKeyEvent(e)) {
        return true;
    }
    else {
        return IsDigitEvent(e);
    }
}

function denyNonDateChars(evnt) {
    var allow = false;
    var keyCode = GetCodeFromEvent(evnt);
    if (keyCode != null) {
        if (IsDigitEvent(evnt) ||
                keyCode == 37  || keyCode == 39  ||     // left-right arrows
                keyCode == 8   || keyCode == 46  ||     // bspc and del
                keyCode == 191 || keyCode == 111 ||     // forward slash
                keyCode == 16  || keyCode == 9) {       // shift and tab
            allow = true;
        }
    }
    return allow;
}

function TabIfLen(txtBox, e, maxLen, nextCtlName, ignoreChar) {
    var curLen;
    var returnVal;

    returnVal = '';

    try {
        if (!IsSpecialKeyEvent(e)) {
            curLen = txtBox.value.length;
            if (ignoreChar && ignoreChar !== '') {
                curLen = replaceAll(txtBox.value, ignoreChar, '').length;
            }

            if (curLen >= maxLen) {
                returnVal = nextCtlName;
            }
        }
    }
    catch (genEx) { }

    try {
        if (returnVal != '') {
            document.getElementById(returnVal).focus();
        }
    }
    catch (tabEx) { }

    return returnVal;
}

function GetCodeFromEvent(e) {
    var code;
    var eventArg;

    code = '';
    eventArg = e;

    if (!eventArg) {
        eventArg = window.event;
    }

    if (eventArg.keyCode) {
        code = eventArg.keyCode;
    }
    else {
        if (eventArg.which) {
            code = eventArg.which;
        }
    }

    if (!code) {
        code = '';
    }

    return code;
}

function GetLetterFromEvent(e) {
    var returnVal;

    try {
        returnVal = String.fromCharCode(GetCodeFromEvent(e));
    }
    catch (err) {
        returnVal = '';
    }

    return returnVal;
}

function isEnterKey(e) {
    var isEnter = false;
    var code = GetCodeFromEvent(e);
    if (code && String(code) === '13') {
        isEnter = true;
    }
    return isEnter;
}

function IsSpecialKeyEvent(e) {
    var code, returnVal;

    code = GetCodeFromEvent(e);
    returnVal = IsSpecialKeyCode(code);
    return returnVal;
}

function IsSpecialKeyCode(charCode) {
    var codeAsInt, returnVal;

    returnVal = false;

    try {
        if (charCode != '') {
            codeAsInt = parseInt(charCode);
            if (codeAsInt <= 31 || codeAsInt > 126 || codeAsInt == 46) {
                returnVal = true;
            }

            if (codeAsInt >= 37 && codeAsInt <= 40) {
                // arrow keys
                returnVal = true;
            }
        }
    }
    catch (err) { }

    return returnVal;
}

function IsDigitEvent(e) {
    var code;
    var returnVal;

    returnVal = false;

    code = GetCodeFromEvent(e);
    returnVal = IsDigitKeyCode(code);

    return returnVal;

}

function IsDigitKeyCode(code) {
    var codeAsInt;
    var returnVal;

    returnVal = false;

    try {
        if (code != '') {
            codeAsInt = parseInt(code);
            if ((codeAsInt >= 48 && codeAsInt <= 57) || (codeAsInt >= 96 && codeAsInt <= 105)) {
                returnVal = true;
            }
        }
    }
    catch (err) { }

    return returnVal;
}

function GetKeyCodeAsInt(e) {
    var code;
    var eventArg;

    code = '';
    eventArg = e;

    if (!eventArg) {
        eventArg = window.event;
    }

    if (eventArg.keyCode) {
        code = eventArg.keyCode;
    }
    else {
        if (eventArg.which) {
            code = eventArg.which;
        }
    }

    if (!code) {
        code = '';
    }

    if (isNaN(code)) {
        code = '0';
    }

    return parseInt(code);
}

function FocusControl(ctlName) {
    try {
        document.getElementById(ctlName).focus();
    }
    catch (err) { }
}