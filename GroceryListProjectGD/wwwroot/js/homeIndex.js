/*
    This is the Home Index Javascript file.
    It sets up the notification system for the home page and allows us to send notifications
    Based on actions that happen on the page.
    This file updates the grocery list page automatically and listens for when a grocery list is created or deleted.
    If a grocery list is created or deleted it sends a notification to users stating what happened.
 */
'use strict';
(function _homeIndexMain() { 
  
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/GroceryHub")
        .build();  

    connection.on("Notification", (message) => {
        const incoming = JSON.parse(message);
        console.log(incoming);
        if (incoming.type === "GROCERY-LIST-CREATED") {
            _updateGroceryTable(incoming.data);
            $("#messageArea").html("A new grocery list was added");
            $('#alertArea').show();
        };

        if (incoming.type === "GROCERY-LIST-DELETED") {
            let rowToRemove = "#row-" + incoming.data;
            $(rowToRemove).fadeOut();
            $('#tbody-grocerylist').remove(rowToRemove);
            $('#messageArea').html("A grocery list was deleted!");
            $('#alertArea').show(400);
        };
    });

    connection.start().catch((err) => {
            return console.error(err.toString());
    });

    _setupPopOvers();

    const createGroceryForm = document.querySelector("#createGroceryListForm");


    createGroceryForm.addEventListener('submit', (e) => {
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
        const url = createGroceryForm.getAttribute('action');
        const method = createGroceryForm.getAttribute('method');
        const formData = new FormData(createGroceryForm);
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
                    console.log('Success: the grocery list was created');
                    $('#createGroceryListModal').modal('hide');
                    _notifyConnectedClients("GROCERY-LIST-CREATED", result.groceryID)
                    window.location.replace("/home/index"); 
                                      
                }
                else {
                   _reportErrors(result);
                }
            })
            
    }

    function _updateGroceryTable(groceryID) {
        fetch(`/Grocery/GroceryRow/${groceryID}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('There was a network error!');
                }
                return response.text();
            })
            .then(result => {
                console.log(result);
                
                $("#tbody-grocerylist").append(result);
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
                if (result?.message === "deleted") {
                    console.log('Success: the grocery list was deleted');
                    _notifyConnectedClients("GROCERY-LIST-DELETED", result.id);
                    window.location.replace("/home/index"); 
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