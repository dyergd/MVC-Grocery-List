/*
    This is the Go Shopping Javascript file.
    It sets up the notification system for the go shopping page and allows us to send notifications
    Based on actions that happen on the page.
    This file listens for when the an item is checked and then listens for when it's unchecked.
    If the item has been checked or unchecked a notification is sent to users stating what happened.
 */
'use strict'
    (function _goShoppingIndexMain() {

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/GroceryHub")
            .build();

        connection.on("Notification", (message) => {
            const incoming = JSON.parse(message);
            console.log(incoming);
            if (incoming.type === "ITEM-CHECKED") {
                _updateItemTable(incoming.data);
                $("#messageArea").html("A new item was checked");
                $('#alertArea').show();
            };

            if (incoming.type === "ITEM-UNCHECKED") {
                let rowToRemove = "#row-" + incoming.data;
                $(rowToRemove).fadeOut();
                $('#tbody-item').remove(rowToRemove);
                $('#messageArea').html("A item was unchecked");
                $('#alertArea').show();

            };
        });

        connection.start().catch((err) => {
            return console.error(err.toString());
        });

        $('#checkboxNoLabel').on('click' , (e)=> {
            document.getElementById('#label').innerHTML = "<del>@Html.DisplayFor(modelItem => item.Name)</del>";
            console.log('Success: the item was checked');
            _notifyConnectedClients("ITEM-CHECKED", result.id);
            $('#checkboxNoLabel').on('click', (e) => {
                document.getElementById('#label').innerHTML = "@Html.DisplayFor(modelItem => item.Name)";
                console.log('Success: the item was unchecked');
                _notifyConnectedClients("ITEM-UNCHECKED", result.id);
            })
        });



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