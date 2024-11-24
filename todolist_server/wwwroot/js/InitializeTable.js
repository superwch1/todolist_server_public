$(function () {
    InitializeModifyButton(".task-button-modify");
    InitializeDoubleClickToModify(".task-row");

    InitializeClickToChangeStatus(".task-int-symbol");

    InitializeSaveButton(".task-button-save");
    InitializeDeleteButton(".task-button-delete");

    CreateTempTaskButton('new-my-task');
    CreateTempTaskButton('new-followup-task');

    InitializeDragAndDrop();
    HideTableBody('.task-type');
})

