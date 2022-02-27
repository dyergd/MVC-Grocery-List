/*
    This is the Permission Index Javascript file.
    It sets up the notification system for the permssion page and allows us to send notifications
    Based on actions that happen on the page.
    This file updates the permission list automatically and listens for when a permission is granted or revoked.
    If a permission is granted or revoked it sends a notification to users stating what happened.
 */
'use strict';
(function _permissionIndexMain() { 

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/GroceryHub")
        .build();

    connection.on("Notification", (message) => {
        const incoming = JSON.parse(message);
        console.log(incoming);
        if (incoming.type === "PERMISSION-CREATED") {
            _updatePermissionTable(incoming.data);
            $("#messageArea").html("A new permission was added");
            $('#alertArea').show();
        };

        if (incoming.type === "PERMISSION-REVOKED") {
            let rowToRemove = "#row-" + incoming.data;
            $(rowToRemove).fadeOut();
            $('#tbody-grocerylist').remove(rowToRemove);
            $('#messageArea').html("A permission was revoked!");
            $('#alertArea').show(400);
        };
    });

    connection.start().catch((err) => {
        return console.error(err.toString());
    });

    _setupPopOvers();

    const grantPermissionForm = document.querySelector("#addPermissonForm");


    grantPermissionForm.addEventListener('submit', (e) => {
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
        const url = grantPermissionForm.getAttribute('action');
        const method = grantPermissionForm.getAttribute('method');
        const formData = new FormData(grantPermissionForm);
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
                if (result?.message === "granted") {
                    console.log('Success: permission was granted');
                    $('#addPermissonModal').modal('hide');
                    _notifyConnectedClients("PERMISSION-CREATED", result.email);
                    window.location.replace("/home/index"); 
                }
                else {
                    _reportErrors(result);
                }
            })

    }

    function _updatePermissionTable(Id) {
        fetch(`/Grocery/PermissionRow/${Id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('There was a network error!');
                }
                return response.text();
            })
            .then(result => {
                console.log(result);
                $("#tbody-permission").append(result);
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

    function _sendDeleteAjax(url, groceryId) {
        fetch(url, {
            method: "post",
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `id=${groceryId}`
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('There was a network error!');
                }
                return response.json();
            })
            .then(result => {
                if (result?.message === "revoke") {
                    console.log('Success: permission was revoked');
                    _notifyConnectedClients("PERMISSION-REVOKED", result.email);
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

    function _notifyConnectedClients(type, data) {
        let message = {
            type, data
        };
        console.log(JSON.stringify(message));
        connection.invoke("SendMessageToAllAsync", JSON.stringify(message))
            .catch(function (err) {
                return console.error(err.toString());
            });
    }
})();