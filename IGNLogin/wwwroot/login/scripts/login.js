function User(id, name, userId, active, token) {
    this.id = id;
    this.name = name;
    this.userId = userId;
    this.active = ko.observable(active);
    this.inactive = ko.observable(!this.active());
    this.token = token;

    this.activate = function () {

        var isActive = false;
        var bearerString = "Bearer " + this.token;
        jQuery.ajax({
            url: "/api/user/activateById?id=" + this.userId,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            async: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", bearerString);
            },
            success: function (data) {
                isActive = true;
            },
            error(request, status, error) {
                console.log(request);
                console.log(status);
                console.log(error);
            }
        });
        this.active(isActive);
        this.inactive(!isActive);
    };

    this.deactivate = function () {

        var isActive = true;
        var bearerString = "Bearer " + this.token;
        jQuery.ajax({
            url: "/api/user/deactivateById?id=" + this.userId,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            async: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", bearerString);
            },
            success: function (data) {
                isActive = false;
            },
            error(request, status, error) {
                console.log(request);
                console.log(status);
                console.log(error);
            }
        });
        this.active(isActive);
        this.inactive(!isActive);
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

    this.uName = ko.observable(localStorage.getItem("un"));

    this.eMail = ko.observable(localStorage.getItem("em"));

    this.password = ko.observable(localStorage.getItem("pw"));

    this.admCode = ko.observable(localStorage.getItem("ac"));

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
                url: "/api/community/list",
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
                        users.push(new User(data[index].id, data[index].login, data[index].userId, data[index].isActive, result.token()));
                    }
                    console.log(data);
                },
                error: function (request, status, error) {
                    console.log(request.responseText);
                    console.log(status);
                    console.log(error);
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
            localStorage.clear();
            localStorage.setItem("un", this.uName());
            localStorage.setItem("em", this.eMail());
            localStorage.setItem("pw", this.password());
            localStorage.setItem("ac", this.admCode());
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
            url: "/api/community/list",
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
                    users.push(new User(data[index].id, data[index].login, data[index].userId, data[index].isActive, result.token()));
                }
                console.log(data);
            },
            error: function (request, status, error) {
                console.log(request.responseText);
                console.log(status);
                console.log(error);
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
        localStorage.clear();
        localStorage.setItem("un", this.uName());
        localStorage.setItem("em", this.eMail());
        localStorage.setItem("pw", this.password());
        localStorage.setItem("ac", this.admCode());
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
        localStorage.clear();
    }
};

$(document).ready(function () {
    var user = new AuthorisationUser();
    ko.applyBindings(user);
});