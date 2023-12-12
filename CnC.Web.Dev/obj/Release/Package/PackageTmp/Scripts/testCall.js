function TestCall()
{ }
TestCall.prototype = {
    TestCallForDetail: function () {
        var self = this;
        var common = new Common();
        console.log("in");
        common.AjaxCall("/Customer/GetCustmersList",
        "",
        "POST", "json", "application/json; charset=utf-8",
        "TestCallForDetailSuccessMessage", "TestCallForDetailErrorMessage", self);

    },
    TestCallForDetailSuccessMessage: function (data) {
        var common = new Common();
        console.log(data);    
    },
    TestCallForDetailErrorMessage: function (data) {
        console.log(data);
    },
}