"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/appHub")
    .build();

connection.on("LoadAppointments", function () {
    location.href = '/Appointments/Index';
});
connection.on("LoadUsers", function () {
    location.href = '/Users/Index';
});
connection.on("LoadDoctorLeafs", function () {
    location.href = '/DoctorLeafs/Index';
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
