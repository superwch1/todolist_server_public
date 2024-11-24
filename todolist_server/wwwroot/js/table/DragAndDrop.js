function InitializeDragAndDrop() {
    $('.task-row').draggable({
        helper: "clone",
        cursor: "move"
    });


    $('#new-my-task-div').droppable({

        drop: async function (event, ui) {
            let id = ui.helper.data('index');

            if ($(`#${id}-taskRow`).data('inttype') !== 0) {

                let topic = $(`#${id}-topic`).text();
                let content = $(`#${id}-content`).html().replaceAll('&amp;', '&').replaceAll('&lt;', '<').replaceAll('&gt;', '>').replaceAll('<br>', '').replaceAll('<div>', '\n').replaceAll('</div>', '');
                content = content.trim();
                let intsymbol = $(`#${id}-intSymbol`).text() === "✔" ? 1 : 0;

                let splitTime = $(`#${id}-dueDate`).val().split('-');
                duedate = splitTime[2] + '-' + splitTime[1] + '-' + splitTime[0];

                await connection.invoke("UpdateTask", id, 0, topic, content, duedate, intsymbol);
            }
        }
    });


    $('#new-followup-task-div').droppable({

        drop: async function (event, ui) {
            let id = ui.helper.data('index');

            if ($(`#${id}-taskRow`).data('inttype') === 0) {

                let topic = $(`#${id}-topic`).text();
                let content = $(`#${id}-content`).html().replaceAll('&amp;', '&').replaceAll('&lt;', '<').replaceAll('&gt;', '>').replaceAll('<br>', '').replaceAll('<div>', '\n').replaceAll('</div>', '');
                content = content.trim();
                let intsymbol = $(`#${id}-intSymbol`).text() === "✔" ? 1 : 0;

                let splitTime = $(`#${id}-dueDate`).val().split('-');
                duedate = splitTime[2] + '-' + splitTime[1] + '-' + splitTime[0];

                await connection.invoke("UpdateTask", id, 1, topic, content, duedate, intsymbol);
            }
        }
    });
}