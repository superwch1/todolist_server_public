var connection;
//set reconnected or build another connection, reload everything
$(function () {
    InitializeSignalR();

    async function InitializeSignalR() {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: context => 5000
            })
            .build();

        connection.keepAliveIntervalInMilliseconds = 10000;
        connection.serverTimeoutInMilliseconds = 20000;

        await connection.start();

        connection.on("DeleteTask", function (id) {
            $(`#${id}-taskRow`).remove();
        });

        connection.on("DeleteThenCreateTask", function (id, intType, topic, content, dueDate, intSymbol) {

            $(`#${id}-taskRow`).remove();

            AddTableRow(id, intType, topic, content, dueDate, intSymbol, "NewTask");

            $(`#${id}-taskRow`).draggable({
                helper: "clone",
                cursor: "move"
            });
        });

        connection.onreconnected(async connectionId => {
            await CounterCheckTask();
        });

        //create the checking function only if it is connected
        if (connection.state == "Connected") {
            //check the connection every 5 seconds and build new connection if it is disconnected
            var reconnectInterval = setInterval(async function () {

                if (connection.state == "Disconnected") {
                    await initializeSignalR(connection);

                    if (connection.state == "Connected") {
                        await CounterCheckTask();
                        clearInterval(reconnectInterval);
                    }
                }
            }, 5000)
        }
    }


    async function CounterCheckTask() {
        let splitTime = $('#selectedTime').val().split('-');

        $.ajax({
            url: `${window.location.origin}/Web/CounterCheckTask?month=${splitTime[1]}&year=${splitTime[0]}`,
            type: 'GET',
            success: function (data) {

                var divIds = [];
                $('.tbody > div').each(function () {
                    var id = $(this).data('index');
                    if (id) {
                        divIds.push(id);
                    }
                });

                for (var i = 0; i < data.length; i++) {
                    for (var j = 0; j < divIds.length; j++) {
                        if (data[i]['id'] === divIds[j]) {
                            let id = divIds[j];

                            //js assume the type of value in data-modify is boolean
                            if ($(`#${id}-taskRow`).data('modify') === false) {

                                if (data[i]['topic'] !== $(`#${id}-topic`).text()) {
                                    $(`#${id}-topic`).text(data[i]['topic']);
                                }

                                let content = $(`#${id}-content`).html().replaceAll('&amp;', '&').replaceAll('&lt;', '<').replaceAll('&gt;', '>').replaceAll('<br>', '').replaceAll('<div>', '\n').replaceAll('</div>', '');
                                content = content.trim();
                                if (data[i]['content'] !== content) {
                                    $(`#${id}-content`).text(data[i]['content']);
                                }

                                let dueDateInDate = new Date(data[i]['dueDate']);
                                let selectedDateInDate = new Date($(`#${id}-dueDate`).val());

                                let dueDateInString = `${dueDateInDate.getFullYear()}-${(dueDateInDate.getMonth() + 1).toString().padStart(2, '0')}-${dueDateInDate.getDate().toString().padStart(2, '0')}`;

                                if (dueDateInDate.getFullYear() !== selectedDateInDate.getFullYear() || dueDateInDate.getMonth() !== selectedDateInDate.getMonth() ||
                                    dueDateInDate.getDate() !== selectedDateInDate.getDate()) {
                                    $(`#${id}-dueDate`).val(dueDateInString);
                                }

                                let symbol = (data[i]['intSymbol'] === 0) ? '✖' : '✔';
                                if (symbol !== $(`#${id}-intSymbol`).text()) {
                                    $(`#${id}-intSymbol`).text(symbol)
                                }

                                if (data[i]['intType'] !== $(`#${id}-taskRow`).data('inttype')) {
                                    $(`#${id}-taskRow`).remove();
                                    AddTableRow(id, data[i]['intType'], data[i]['topic'], data[i]['content'], dueDateInString, data[i]['intSymbol'], "NewTask");
                                }
                            }

                            data.splice(i, 1);
                            divIds.splice(j, 1);

                            //when you delete an element in index 10, element at index 11 move forward to index 10
                            i--;

                            break;
                        }
                    }
                }

                for (var i = 0; i < data.length; i++) {
                    let dueDateInDate = new Date(data[i]['dueDate']);
                    let dueDateInSting = `${dueDateInDate.getFullYear()}-${(dueDateInDate.getMonth() + 1).toString().padStart(2, '0')}-${dueDateInDate.getDate().toString().padStart(2, '0')}`;

                    AddTableRow(data[i]['id'], data[i]['intType'], data[i]['topic'], data[i]['content'], dueDateInSting, data[i]['intSymbol'], "NewTask");
                }


                for (var i = 0; i < divIds.length; i++) {

                    //js assume the type of value in data-modify is boolean
                    if ($(`#${divIds[i]}-taskRow`).data('modify') === false) {
                        $(`#${divIds[i]}-taskRow`).remove();
                    }
                }

            },
            error: function (error) {
            }
        });
    }
})


