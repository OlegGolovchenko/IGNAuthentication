function User(id, name, active, token) {
    this.id = id;
    this.name = name;
    this.active = active;
    this.inactive = !this.active;
    this.token = token;

    this.activate = function () {

        var isActive = false;
        var bearerString = "Bearer " + this.token;
        jQuery.ajax({
            url: "/api/user/activateById?id=" + this.id,
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", bearerString);
            },
            success: function (data) {
                isActive = true;
            }
        });
        this.active = isActive;
    };

    this.deactivate = function () {

        var isActive = true;
        var bearerString = "Bearer " + this.token;
        jQuery.ajax({
            url: "/api/user/deactivateById?id=" + this.id,
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", bearerString);
            },
            success: function (data) {
                isActive = false;
            }
        });
        this.active = isActive;
    };
};

function CommunityUser() {
    this.id = ko.observable();
    this.name = ko.observable();
    this.email = ko.observable();
    this.active = ko.observable();
    this.error = ko.observable();
};

function UserModel(login, email, token) {
    this.login = ko.observable(login);
    this.eMail = ko.observable(email);
    this.token = ko.observable(token);
};

function AuthorisationUser() {
    this.registerRequested = ko.observable(false);

    this.uName = ko.observable("");

    this.eMail = ko.observable("");

    this.password = ko.observable("");

    this.admCode = ko.observable("");

    this.registeredUsers = ko.observableArray();

    this.selectedUser = ko.observable();

    this.loggedInUser = ko.observable(new UserModel());

    this.logInNeeded = ko.observable(true);

    this.loggedIn = ko.observable(false);

    this.loggedInAdmin = ko.observable(false);

    this.register = function () {
        if (!this.registerRequested()) {
            this.registerRequested(true);
        }
        else {
            var result = new UserModel("", "", "");
            var isAdmin = false;
            jQuery.ajax({
                url: "/api/community/register",
                type: "POST",
                data: JSON.stringify({
                    Email: this.eMail(),
                    Password: this.password(),
                    AdminCode: this.admCode(),
                    Login: this.uName()
                }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                success: function (data) {
                    result = new UserModel(data.login, data.email, data.token);
                }
            });
            jQuery.ajax({
                url: "/api/community/isadmin",
                type: "GET",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", "Bearer " + result.token);
                },
                success: function (data) {
                    isAdmin = data;
                    console.log(data);
                }
            });
            var users = new Array();
            jQuery.ajax({
                url: "/api/community",
                type: "GET",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", "Bearer " + result.token());
                },
                success: function (data) {
                    var index = 0;
                    for (index = 0; index < data.length; index++) {
                        users.push(new User(data[index].id, data[index].login, data[index].isActive, result.token()));
                    }
                    console.log(data);
                }
            });
            for (var i = 0; i < users.length; i++) {
                this.registeredUsers.push(users[i]);
            }
            this.loggedInAdmin(isAdmin);
            this.loggedInUser(result);
            this.logInNeeded(false);
            this.registerRequested(false);
            this.loggedIn(true);
            this.registerRequested(false);
        }
    };

    this.login = function () {
        var result = new UserModel("", "", "");
        jQuery.ajax({
            url: "/api/community/login",
            type: "POST",
            data: JSON.stringify({
                Email: this.eMail(),
                Password: this.password(),
                AdminCode: this.admCode()
            }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (data) {
                result = new UserModel(data.login, data.email, data.token);
            }
        });
        jQuery.ajax({
            url: "/api/community/isadmin",
            type: "GET",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + result.token());
            },
            success: function (data) {
                isAdmin = data;
            }
        });
        var users = new Array();
        jQuery.ajax({
            url: "/api/community",
            type: "GET",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            async: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + result.token());
            },
            success: function (data) {
                var index = 0;
                for (index = 0; index < data.length; index++) {
                    users.push(new User(data[index].id, data[index].login, data[index].isActive, result.token()));
                }
                console.log(data);
            }
        });
        for (var i = 0; i < users.length; i++) {
            this.registeredUsers.push(users[i]);
        }
        this.loggedInAdmin(isAdmin);
        this.loggedInUser(result);
        this.logInNeeded(false);
        this.registerRequested(false);
        this.loggedIn(true);
    };

    this.dlOffActivator = function () {
        window.open("/api/user/offkeygen.zip", '_blank');
    };

    this.logout = function () {
        this.loggedInUser(new UserModel("", "", ""));
        this.registeredUsers.removeAll();
        this.logInNeeded(true);
        this.registerRequested(false);
        this.loggedIn(false);
        this.loggedInAdmin(false);
    }
};

$(document).ready(function () {
    var user = new AuthorisationUser();
    ko.applyBindings(user);
});