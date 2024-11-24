function AddTableRow(id, intType, topic, content, dueDate, intSymbol, type) {

    let symbol = (intSymbol === 0) ? '✖' : '✔';
    let placeholder = type === "TempTask" ? "placeholder" : "";
    let editable = type === "TempTask" ? "true" : "false";
    let dateReadonly = (type === "TempTask") ? "" : "readonly";
    let hideSaveButton = (type === "TempTask") ? "" : "d-none";

    let topicHTML = `<div id="${id}-topic" data-text="[topic]" class="task-topic px-3 ${placeholder}" contenteditable="${editable}">${topic}</div>`;
    let contentHTML = `<div id="${id}-content" data-text="[content]" class="task-content ${placeholder}" contenteditable="${editable}">${content}</div>`;
    let intSymbolHTML = `<div id="${id}-intSymbol" data-index="${id}" class="text-center task-int-symbol">${symbol}</div>`;
    let dueDateHTML = `<div class="task-due-date"><input id="${id}-dueDate" class="text-center" type="date" value="${dueDate}" ${dateReadonly} /></div>`;

    let modifyButtonHTML = `<img id="${id}-modify" data-index="${id}" class="task-button-modify task-icon mr-1" src="/lib/table-icon/static-modify.png" />`;
    let saveButtonHTML = `<img id="${id}-save" data-index="${id}" class="task-button-save task-icon mr-1 ${hideSaveButton}" src="/lib/table-icon/static-save.png" />`;
    let deleteButtonHTML = `<img id="${id}-delete" data-index="${id}" class="task-button-delete task-icon" src="/lib/table-icon/static-delete.png" />`;

    let optionHTML;
    if (type === "TempTask") {
        optionHTML = `<div class="text-center task-button">` + saveButtonHTML + deleteButtonHTML + `</div>`;
    }
    else if (type === "NewTask") {
        optionHTML = `<div class="text-center task-button">` + modifyButtonHTML + saveButtonHTML + deleteButtonHTML + `</div>`;
    }

    let selectedTime = $('#selectedTime').val().split('-');

    let currentDate = new Date();
    let dueDateInDate = new Date(dueDate);
    if (selectedTime[0] != dueDateInDate.getFullYear() || selectedTime[1] != dueDateInDate.getMonth() + 1) {
        return;
    }

    let backgroundColor = "";

    if (type === "TempTask") {
        backgroundColor = '#d9ad73';
    }
    else if (type === "NewTask") {

        let previousDate = currentDate.setDate(currentDate.getDate() - 1);

        if (intSymbol === 0 && dueDateInDate < previousDate) {
            backgroundColor = '#f5c6cb';
        }
        else {
            if (intType === 0) {
                backgroundColor = '#d9e6d3';
            }
            else {
                backgroundColor = '#d3dee6';
            }
        }
    }

    let modify = "false";
    if (type === "TempTask") {
        modify = "true";
    }

    let row = `<div id="${id}-taskRow" data-intType="${intType}" data-index="${id}" data-modify="${modify}" class='row task-row m-0 mb-3 px-1 py-2'style="background-color:${backgroundColor}">`
        + topicHTML + contentHTML + intSymbolHTML + dueDateHTML + optionHTML + '</div>';

                                 
    let tbodyId = (intType === 0) ? "new-my-task-tbody" : "new-followup-task-tbody";
    
    if (type === "TempTask") {
        $(`#${tbodyId}`).prepend(row);
    }

    else if (type === "NewTask") {
        let tableRowIds = [];

        $(`#${tbodyId} > div`).each(function () {
            let trId = $(this).attr('id');
            if (!trId.includes("new")) {
                tableRowIds.push($(`#${trId}`).data("index"));
            }
        });

        if (tableRowIds.length === 0) {
            $(`#${tbodyId}`).append(row);
        }
        else if (tableRowIds.length > 0) {
            tableRowIds.sort(function (a, b) {

                let symbolA = $(`#${a}-intSymbol`).text();
                let symbolB = $(`#${b}-intSymbol`).text();

                let dateA = new Date($(`#${a}-dueDate`).val());
                let dateB = new Date($(`#${b}-dueDate`).val());

                if (symbolA !== symbolB) {
                    return symbolA === '✖' ? -1 : 1;
                }

                return dateA - dateB;
            });

            let inserted = false;

            for (let i = 0; i < tableRowIds.length; i++) {
                let rowSymbol = $(`#${tableRowIds[i]}-intSymbol`).text();
                let rowDate = new Date($(`#${tableRowIds[i]}-dueDate`).val());

                if (symbol === '✔' && rowSymbol === '✖') {
                    continue;
                }

                if ((symbol === '✖' && rowSymbol === '✔') || dueDateInDate < rowDate) {
                    $(`#${tableRowIds[i]}-taskRow`).before(row);
                    inserted = true;
                    break;
                }
            }

            if (!inserted) {
                $(`#${tbodyId} > div:last`).after(row);
            }
        } 
    }

    if (type === "TempTask") {
        $(`#${id}-table`).removeClass("d-none");

        $(`#${id}-intSymbol`).on('click', function () {
            let symbol = $(this).text() === '✔' ? '✖' : '✔';
            $(this).text(symbol);
        });
        $(`#${id}-intSymbol`).css('cursor', 'pointer');
        $(`#${id}-intSymbol`).css('user-select', 'none');
    }


    if (type === "TempTask") {
        InitializeDeleteButtonTempTask(`#${id}-delete`);
        InitializeSaveButtonTempTask(`#${id}-save`);
    }
    else if (type === "NewTask") {
        InitializeModifyButton(`#${id}-modify`);
        InitializeDoubleClickToModify(`#${id}-taskRow`);
        InitializeClickToChangeStatus(`#${id}-intSymbol`);
        InitializeDeleteButton(`#${id}-delete`);
        InitializeSaveButton(`#${id}-save`);
    }
}