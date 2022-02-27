/*
    This is the Edit Index Javascript file.
    It sets up the notification system for the edit page and allows us to send notifications
    Based on actions that happen on the page.
    This file updates the item list on the edit page automatically and listens for when the grocery list name is changed or if a item is deleted.
    If the grocery list name if edited or if a item had been deletde it sends a notification to users stating what happened.
 */
'use strict';
(function _editIndexMain() { 

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/GroceryHub")
        .build();

    connection.on("Notification", (message) => {
        const incoming = JSON.parse(message);
        console.log(incoming);
        if (incoming.type === "ITEM-CREATED") {
            _updateItemTable(incoming.data);
            $("#messageArea").html("A new item was added");
            $('#alertArea').show();
        };

        if (incoming.type === "ITEM-DELETED") {
            let rowToRemove = "#row-" + incoming.data;
            $(rowToRemove).fadeOut();
            $('#tbody-item').remove(rowToRemove);
            $('#messageArea').html("A item was deleted!");
            $('#alertArea').show();

        };
    });

    connection.start().catch((err) => {
        return console.error(err.toString());
    });

    _setupPopOvers();

    const createItemForm = document.querySelector("#itemCreationForm");


    createItemForm.addEventListener('submit', (e) => {
        e.preventDefault();
        _clearErrorMessages();
        _submitWithAjax();
    });

    function _clearErrorMessages() {
        $.each($('span[data-valmsg-for]'), function _clearSpan() {
            $(this).html("");
        });
    };

    function _submitWithAjax() {
        const url = createItemForm.getAttribute('action');
        const method = createItemForm.getAttribute('method');
        const formData = new FormData(createItemForm);
        console.log(`${url} ${method}`);

        fetch(url, {
            method: method,
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('There was a network error!');
                }
                return response.json();
            })
            .then(result => {
                if (result?.message === "created") {
                    console.log('Success: the item was created');
                    $('#itemCreationModal').modal('hide');
                    _notifyConnectedClients("ITEM-CREATED", result.id)

                }
                else {
                    _reportErrors(result);
                }
            })

    }

    function _updateItemTable(itemId) {
        fetch(`/Grocery/ItemRow/${itemId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('There was a network error!');
                }
                return response.text();
            })
            .then(result => {
                console.log(result);
                $("#tbody-edit").append(result);
                _setupPopOvers();

            })
    }


    function _setupPopOvers() {
        $('[data-toggle="popover"]').popover();
        $('.popover-dismiss').popover({ trigger: 'focus' });
        $('[data-toggle="popover"]').on('inserted.bs.popover', function _onPopoverInserted() {
            let $a = $(this);
            let url = $a.attr('href');
            let idx = url.lastIndexOf('/');
            let id = url.substring(idx + 1);
            url = url.substring(0, idx);
            let btnYesId = "btnYes-" + id;
            $('.popover-body').html(`<button id="${btnYesId}">Yes</button><button>No</button>`);
            $(`#${btnYesId}`).on('click', function _onYes() {
                console.log(url);
                console.log(id);
                _sendDeleteAjax(url, id);
            });
        });

    }

    function _sendDeleteAjax(url, itemId) {
        fetch(url, {
            method: "post",
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `id=${itemId}`
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('There was a network error!');
                }
                return response.json();
            })
            .then(result => {
                if (result?.message === "deleted") {
                    console.log('Success: the item was deleted');
                    _notifyConnectedClients("ITEM-DELETED", result.id);
                }
                else {
                    _reportErrors(result);
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }

    $('#alertCloseBtn').on('click', function _hideAlert() {
        $('#alertArea').hide();
    });

    $(document).on('click', '.deleteAjax', (e) => {
        e.preventDefault();
    });



    function _reportErrors(response) {
        for (let key in response) {
            if (response[key].errors.length > 0) {
                for (let error of response[key].errors) {
                    console.log(key + " : " + error.errorMessage);
                    const selector = `span[data-valmsg-for="${key}"]`
                    const errMessageSpan = document.querySelector(selector);
                    if (errMessageSpan !== null) {
                        errMessageSpan.textContent = error.errorMessage;
                    }
                }
            }
        }
    }

    
})();