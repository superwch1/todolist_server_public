function InitializeModifyButton(idOrClass) {
    $(idOrClass).on('click', function () {
        let id = $(this).data("index");

        $(`#${id}-modify`).addClass('d-none');
        $(`#${id}-save`).removeClass('d-none');

        $(`#${id}-intSymbol`).off('click');
        $(`#${id}-intSymbol`).on('click', function () {
            let symbol = $(`#${id}-intSymbol`).text() === '✔' ? '✖' : '✔';
            $(`#${id}-intSymbol`).text(symbol);
        });

        $(`#${id}-topic`).attr('contenteditable', 'true');
        $(`#${id}-topic`).addClass('placeholder');

        $(`#${id}-content`).attr('contenteditable', 'true');
        $(`#${id}-content`).addClass('placeholder');

        $(`#${id}-dueDate`).attr('readonly', false);

        $(`#${id}-taskRow`).css('background-color', '#d9ad73');
        $(`#${id}-taskRow`).draggable('disable');
        $(`#${id}-taskRow`).data('modify', 'true');
    });

    $(idOrClass).hover(
        function () {
            $(this).attr("src", "/lib/table-icon/gif-modify.gif");
        }
    );

    $(idOrClass).mouseout(
        function () {
            $(this).attr("src", "/lib/table-icon/static-modify.png");
        }
    );
}


function InitializeDoubleClickToModify(idOrClass) {
    $(idOrClass).on('dblclick', function () {
        let id = $(this).data("index");

        $(`#${id}-modify`).addClass('d-none');
        $(`#${id}-save`).removeClass('d-none');

        $(`#${id}-intSymbol`).off('click');
        $(`#${id}-intSymbol`).on('click', function () {
            let symbol = $(`#${id}-intSymbol`).text() === '✔' ? '✖' : '✔';
            $(`#${id}-intSymbol`).text(symbol);
        });

        $(`#${id}-topic`).attr('contenteditable', 'true');
        $(`#${id}-topic`).addClass('placeholder');

        $(`#${id}-content`).attr('contenteditable', 'true');
        $(`#${id}-content`).addClass('placeholder');

        $(`#${id}-dueDate`).attr('readonly', false);

        $(`#${id}-taskRow`).css('background-color', '#d9ad73');
        $(`#${id}-taskRow`).draggable('disable');
        $(`#${id}-taskRow`).data('modify', 'true');
    });
}


function InitializeClickToChangeStatus(idOrClass) {
    $(idOrClass).on('click', async function () {

        let id = $(this).data("index");
        let inttype = $(`#${id}-taskRow`).data('inttype');
        let topic = $(`#${id}-topic`).text();
        let content = $(`#${id}-content`).html().replaceAll('&amp;', '&').replaceAll('&lt;', '<').replaceAll('&gt;', '>').replaceAll('<br>', '').replaceAll('<div>', '\n').replaceAll('</div>', '');
        content = content.trim();
        let intsymbol = $(`#${id}-intSymbol`).text() === "✔" ? 0 : 1;

        if (!$(`#${id}-dueDate`).val()) {
            alert('Please enter the due date');
            return;
        }

        if (!$(`#${id}-topic`).text()) {
            alert('Please enter the topic');
            return;
        }

        let splitTime = $(`#${id}-dueDate`).val().split('-');
        duedate = splitTime[2] + '-' + splitTime[1] + '-' + splitTime[0];

        await connection.invoke("UpdateTask", id, inttype, topic, content, duedate, intsymbol);
    });

    $(idOrClass).hover(
        function () {
            $(this).attr("src", "/lib/table-icon/gif-save.gif");
        }
    );

    $(idOrClass).mouseout(
        function () {
            $(this).attr("src", "/lib/table-icon/static-save.png");
        }
    );
}



function InitializeDeleteButton(idOrClass) {
    $(idOrClass).on('click', async function () {
        if (window.confirm("Do you really want to delete the task?")) {
            await connection.invoke("DeleteTask", $(this).data("index"));
        }
    });

    $(idOrClass).hover(
        function () {
            $(this).attr("src", "/lib/table-icon/gif-delete.gif");
        }
    );

    $(idOrClass).mouseout(
        function () {
            $(this).attr("src", "/lib/table-icon/static-delete.png");
        }
    );
}


function InitializeDeleteButtonTempTask(idOrClass) {
   
    $(idOrClass).on('click', function () {
        let id = $(this).data("index");

        if (window.confirm("Do you really want to delete the task?")) {
            $(`#${id}-taskRow`).remove();
            $(`#${id}-button`).css('display', 'initial');
        }
    });

    $(idOrClass).hover(
        function () {
            $(this).attr("src", "/lib/table-icon/gif-delete.gif");
        }
    );

    $(idOrClass).mouseout(
        function () {
            $(this).attr("src", "/lib/table-icon/static-delete.png");
        }
    );
}


function InitializeSaveButton(idOrClass) {
    $(idOrClass).on('click', async function () {

        let id = $(this).data("index");
        let inttype = $(`#${id}-taskRow`).data('inttype');
        let topic = $(`#${id}-topic`).text();
        let content = $(`#${id}-content`).html().replaceAll('&amp;', '&').replaceAll('&lt;', '<').replaceAll('&gt;', '>').replaceAll('<br>', '').replaceAll('<div>', '\n').replaceAll('</div>', '');
        content = content.trim();
        let intsymbol = $(`#${id}-intSymbol`).text() === "✔" ? 1 : 0;

        if (!$(`#${id}-dueDate`).val()) {
            alert('Please enter the due date');
            return;
        }

        if (!$(`#${id}-topic`).text()) {
            alert('Please enter the topic');
            return;
        }

        let splitTime = $(`#${id}-dueDate`).val().split('-');
        duedate = splitTime[2] + '-' + splitTime[1] + '-' + splitTime[0];

        await connection.invoke("UpdateTask", id, inttype, topic, content, duedate, intsymbol);
    });

    $(idOrClass).hover(
        function () {
            $(this).attr("src", "/lib/table-icon/gif-save.gif");
        }
    );

    $(idOrClass).mouseout(
        function () {
            $(this).attr("src", "/lib/table-icon/static-save.png");
        }
    );
}


function InitializeSaveButtonTempTask(idOrClass) {
    $(idOrClass).on('click', async function () {

        let id = $(this).data("index");
        let inttype = $(`#${id}-taskRow`).data('inttype');
        let topic = $(`#${id}-topic`).text();
        let content = $(`#${id}-content`).html().replaceAll('&amp;', '&').replaceAll('&lt;', '<').replaceAll('&gt;', '>').replaceAll('<br>', '').replaceAll('<div>', '\n').replaceAll('</div>', '');
        content = content.trim();
        let intsymbol = $(`#${id}-intSymbol`).text() === "✔" ? 1 : 0;

        if (!$(`#${id}-dueDate`).val()) {
            alert('Please enter the due date');
            return;
        }

        if (!$(`#${id}-topic`).text()) {
            alert('Please enter the topic');
            return;
        }

        let splitTime = $(`#${id}-dueDate`).val().split('-');
        duedate = splitTime[2] + '-' + splitTime[1] + '-' + splitTime[0];

        let response = await connection.invoke("CreateTask", inttype, topic, content, duedate, intsymbol);
        if (response == "Done") {
            $('#' + id + '-taskRow').remove();
            $('#' + id + '-button').css('display', 'initial');
        }
    });

    $(idOrClass).hover(
        function () {
            $(this).attr("src", "/lib/table-icon/gif-save.gif");
        }
    );

    $(idOrClass).mouseout(
        function () {
            $(this).attr("src", "/lib/table-icon/static-save.png");
        }
    );
}

function CreateTempTaskButton(id) {
    $(`#${id}-button`).on('click', function () {
        let intType = (id === 'new-my-task') ? 0 : 1;

        let selectedTime = $('#selectedTime').val().split('-');
        let today = new Date();       
        let dd = "01";
        let mm = selectedTime[1];
        let yyyy = selectedTime[0];
        today = yyyy + '-' + mm + '-' + dd;

        AddTableRow(id, intType, "", "", today, 0, "TempTask");

        $(`#${id}-button`).css('display', 'none');
    });
}


function HideTableBody(idOrClass) {
    $(`${idOrClass}`).on('click', function () {     
        let type = $(this).data('type');

        if ($(`#${type}-table`).is(':visible')) {
            $(`#${type}-hide`).removeClass('d-none');
            $(`#${type}-show`).addClass('d-none');

            $(`#${type}-table`).hide();
        }
        else {
            $(`#${type}-show`).removeClass('d-none');
            $(`#${type}-hide`).addClass('d-none');

            $(`#${type}-table`).show();
        }
    });
}