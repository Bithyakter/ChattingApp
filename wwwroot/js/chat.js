"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;
var gid = 0;
connection.on("RefreshChat", function () {
    Getallmassage(gid, 0);
    Getallgroup();
    Getallnotification();
    Getchatmember();
});
connection.on("RefreshMember", function () {
    Getallmassage(0,0);
    Getallmember();
    Getallgroup();
    Getallnotification();
    GetTotaluser();
    Getchatmember();
});

connection.start().then(function () {
    
    Getallmassage(0,0);
    Getallgroup();
    Getallnotification();
    Getchatmember();
}).catch(function (err) {
    return alert('Can not conect!!!');
});



function Getallmassage(x,y) {
    $.ajax({
        type: "GET",
        url: "/Chat/GetChat",
        dataType: "json",
        data: {'groupid':x,'uid':y},
        success: function (data) {
            var htm = ""

            if (data.length > 0) {
                if (data[0].length > 0) {
                    $('#showname').html(data[0][0].name);
                    $('#showst').html(data[0][0].status);
                    $('#uid').val(data[0][0].userid);
                    $('#gid').val(data[0][0].groupid);
                    $('#showimg').attr("src", data[0][0].images);
                    $('#mtype').val(data[0][0].mytype);
                }
                if (data[1].length > 0) {
                    for (var i = 0; i < data[1].length; i++) {
                        var st = data[1][i].flag == 1 ? 'left' : 'right';
                        var msgg = data[1][i].masssage == null ? '' : data[1][i].masssage;
                        htm += ' ';
                        if (data[1][i].file!=null) {
                            if (data[1][i].flag == 0) {
                                htm += ' <div class="chat chat-right"><div class="chat-body"><div class="chat-bubble">';
                                htm += ' <div class="chat-content img-content"><div class="chat-img-group clearfix"><a class="chat-img-attach d-inline-block">';
                                if (data[1][i].isimg==true) {
                                    htm += ' <img alt="" class="max-w-100" src="' + data[1][i].file + '" href="javascript:void(0)"><div class="chat-meta"><div class="chat-file-name" style="color:black">' + data[1][i].filename + '</div>';
                                    htm += ' <div class="chat-file-size"></div>';
                                }
                                else {
                                    htm += ' <a href="' + data[1][i].file + '"><div class="chat-meta"><div class="chat-file-name" style="color:black">' + data[1][i].filename + '</div> </a>';
                                    htm += ' <div class="chat-file-size"></div>';

                                }
                               
                                htm += ' </div></a></div></div>';
                                htm += ' <div class="chat-content"><p>' + msgg + '</p ><span class="chat-time">' + data[1][i].massagedate + '</span> </div></div></div></div>';
                              
                            }
                            else {
                                htm += ' <div class="chat chat-left"><div class="chat-avatar"><a class="avatar"><img alt="" title="' + data[1][i].firstname + '" src="' + data[1][i].images + '" class="img-responsive img-circle">	</a></div>';
                                htm += ' <div class="chat-body"><div class="chat-bubble">';
                                htm += ' <div class="chat-content img-content"><div class="chat-img-group clearfix"><a class="chat-img-attach d-inline-block">';
                                if (data[1][i].isimg == true) {
                                    htm += ' <img alt="" class="max-w-100" src="' + data[1][i].file + '" href="javascript:void(0)"><div class="chat-meta"><div class="chat-file-name" style="color:black">' + data[1][i].filename + '</div>';
                                    htm += ' <div class="chat-file-size"></div>';
                                }
                                else {
                                    htm += ' <a href="' + data[1][i].file + '" ><div class="chat-meta"><div class="chat-file-name" style="color:black">' + data[1][i].filename + '</div> </a>';
                                    htm += ' <div class="chat-file-size"></div>';

                                }


                                htm += ' </div></a></div></div>';
                                htm += ' <div class="chat-content"><p>' + msgg + '</p><span class="chat-time">' + data[1][i].massagedate + '</span></div></div></div></div>';
                               
                            }
                        } 
                                    
                                    
                        else {
                            if (data[1][i].flag == 0) {
                                htm += ' <div class="chat chat-right"><div class="chat-body"><div class="chat-bubble"><div class="chat-content">';
                                htm += ' <p>' + msgg + '</p ><span class="chat-time">' + data[1][i].massagedate + '</span> </div></div></div></div>';

                            }
                            else {
                                htm += ' <div class="chat chat-left"><div class="chat-avatar"><a class="avatar"><img alt="" title="' + data[1][i].firstname + '" src="' + data[1][i].images + '" class="img-responsive img-circle">	</a></div>';
                                htm += ' <div class="chat-body"><div class="chat-bubble">';                                
                                htm += ' <div class="chat-content"><p>' + msgg + '</p><span class="chat-time">' + data[1][i].massagedate + '</span></div></div></div></div>';
                               
                            }
                        }
                       
                    }
                   
                }
               
               
            }
           
            $('#massage').html(htm);
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function Getallgroup() {
    $.ajax({
        type: "GET",
        url: "/Chat/GetGroup",
        dataType: "json",
        success: function (data) {
            var htm = ""

            if (data!=null) {
                var imgs = 'images/icons/avatar1.png';
                var st = '';
                htm += '  <a href="javascript:void(0)" class="item d-flex" onclick="SetMember(\'' + data.groupname + '\',0,' + data.groupid + ',\'' + st + '\',1,\'' + imgs + '\',' + data.userid + ')";>';
                htm += '  <div class="avatar order-1">';
                htm += ' <img src="images/icons/avatar1.png" alt="avatar" class="rounded-circle w-40"> </div>';
                htm += ' <input type="hidden" value="' + data.groupid + '"/>';
                htm += '  <div class="row-content order-2"><small class="text-muted float-right">Now</small> <p class="list-group-item-title">' + data.groupname + '</p> <p class="list-group-item-text">Public group, Any one can view massage</p></div></a>';
             
            }
            $('#group').html(htm);
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function getUserList(e) {
    var key = e.target.value;
    $.ajax({
        type: "GET",
        url: "/Chat/GetMember",
        data: {key:key},
        dataType: "json",
        success: function (data) {
            var htm = ""

            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                  
                    var st = data[i].status == false ? 'Offline' : 'Online';
                    htm += ' <li>';
                    htm += ' <a href="javascript:void(0)" class="item d-flex" onclick="SetMember(\'' + data[i].name + '\',' + data[i].userid + ',' + data[i].groupid + ',\'' + st + '\',0,\'' + data[i].images + '\',' + data[i].userid + ');">';
                    htm += '  <div class="avatar order-1">';
                    htm += ' <img src="' + data[i].images + '" alt="avatar" class="rounded-circle w-40"></div>';
                    htm += ' <input type="hidden" id="mgid" value="' + data[i].groupid + '"/>';
                    htm += ' <input type="hidden" id="muid" value="' + data[i].userid + '"/>';
                    htm += ' <div class="row-content order-2"><small class="text-muted float-right">' + st + '</small> <p class="list-group-item-title">' + data[i].name + '</p>';
                    htm += '  <p class="list-group-item-text">' + data[i].jobname + ',' + data[i].company + ' </p> </div></a></li>';

                }
                $('#userlistdata').css("display", "block");
            }
            $('#userlistdata').html(htm);
           
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function SetMember(x,y,z,a,b,c) {
   
   
    $('#showname').html(x);
    $('#showst').html(a);
    $('#showimg').attr("src", c);

    $('#uid').val(y);
    $('#gid').val(z);
    $('#mtype').val(b);
    Getallmassage(z,y);
    $('#userlistdata').html('');
}
function Sendmassage() {
   
    var fdata = new FormData();
    var files = $("#myFileInput").get(0).files;
    if (files.length > 0) {
        fdata.append('file', files[0]);
    }
    fdata.append("masssage", $('#messageInput').val());
    fdata.append("userid", $('#uid').val());
    fdata.append("groupid", $('#gid').val());
    fdata.append("mtype", $('#mtype').val());
    $.ajax({
        type: "POST",
        url: "/Chat/SendChat",
        processData: false,
        contentType: false,
        dataType: "json",
        data: fdata,
        success: function (msg) {
            gid = 0;
            $('#messageInput').val("");
            connection.invoke("ShowData").catch(function (err) {
              
                event.preventDefault();
            });
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function Getallnotification() {
    $.ajax({
        type: "GET",
        url: "/Chat/GetNotification",
        dataType: "json",
        success: function (data) {
            var htm = ""

            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                   
                    htm += ' <tr><td>'+i+1+'</td>';
                    htm += ' <td><a href="/Chat/Index">' + data[i].notification +'<a></td>';
                    htm += ' <td>' + data[i].notificationdate + '</td></tr>';
                   
                }

            } $('#notid').html(htm);
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function Getchatmember() {
    $.ajax({
        type: "GET",
        url: "/Chat/GetchatMember",
        dataType: "json",
        success: function (data) {
            var htm = ""

            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var st = data[i].status == false ? 'Offline' : 'Online';
                    var clr = data[i].status == false ? 'gray' : 'green';

                    htm += ' <a href="javascript:void(0)" class="item d-flex" onclick="SetMember(\'' + data[i].name + '\',' + data[i].userid + ',' + data[i].groupid + ',\'' + st + '\',0,\'' + data[i].images + '\',' + data[i].userid + ');">';
                    htm += '  <div class="avatar order-1">';
                    htm += ' <img src="' + data[i].images + '" alt="avatar" class="rounded-circle w-40"></div>';
                    htm += ' <input type="hidden" id="mgid" value="' + data[i].groupid + '"/>';
                    htm += ' <input type="hidden" id="muid" value="' + data[i].userid + '"/>';
                    htm += ' <div class="row-content order-2"><small class="text-muted float-right">' + st + '</small> <p class="list-group-item-title">' + data[i].name + '</p>';
                    htm += '  <p class="list-group-item-text">' + data[i].jobname + ',' + data[i].company + ' </p> </div></a>';

                }
							
            } $('#chatmember').html(htm);
        },
        error: function (e) {
            console.log(e);
        }
    });
}