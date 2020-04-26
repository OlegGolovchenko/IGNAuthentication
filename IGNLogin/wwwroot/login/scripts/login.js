function User(id, name, active) {
    this.id = id;
    this.name = name;
    this.active = active;
    this.inactive = !this.active;
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

    this.users = ko.observableArray([new User(0, "test", true), new User(1, "test2", false)]);

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
        this.loggedInAdmin(isAdmin);
        this.loggedInUser(result);
        this.logInNeeded(false);
        this.registerRequested(false);
        this.loggedIn(true);
    };

    this.dlOffActivator = function() {

    };

    this.logout = function () {
        this.loggedInUser(new UserModel("","",""));
        this.logInNeeded(true);
        this.registerRequested(false);
        this.loggedIn(false);
        this.loggedInAdmin(false);
    }

    this.activate = function () {
        if (this.selectedUser().active) {
            alert("selected user is: " + this.selectedUser().name + " is active");
        } else {
            alert("selected user is: " + this.selectedUser().name);
        }
    };

    this.deactivate = function () {
        if (!this.selectedUser().active) {
            alert("selected user is: " + this.selectedUser().name + " is inactive");
        } else {
            alert("selected user is: " + this.selectedUser().name);
        }
    };
};

$(document).ready(function () {
    var user = new AuthorisationUser();
    ko.applyBindings(user);
});