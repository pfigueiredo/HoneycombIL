//'TimeSheetApp.HttpInterceptor'

if (!window._helper) window._helper = {};
window._helper.getData = function ($scope, $http, url, property, callback) {
    $http.get(url).success(function (response) {
        $scope[property] = response;
        if (callback) callback();
    }).error(function (error) {
        window.alert("Error loading " + url);
    });
}

var app = angular.module('TimeSheetApp', ['smart-table'], function ($locationProvider) {
    $locationProvider.html5Mode({
        enabled: false,
        requireBase: false
    });
}, null);

app.controller('PageController', function ($scope, $http, $location, $timeout) {

    $scope.expressions = [];
    $scope.displayDailyTimeSheet = [];
    $scope.dailyTimeSheet = [];
    $scope.dailyTimeSheet_org = [];
    $scope.bwsTimeSheet = [];
    $scope.syntaxOk = true;

    window._helper.app.angular.buildRepeatingAreaFunctions(
        "Line", $scope,
        function () {
            if (!$scope.expressions) $scope.expressions = [];
            return $scope.expressions;
        }, function() {
            var index = $scope.expressions.length - 1;
            $timeout(function () { $("#var_" + index).focus(); }, 0, false);
        }
    );



    $scope.qEmployeeNum = 22414;
    $scope.qSocId = 8;

    $scope.queryTimeSheet = function () {
        window._helper.getData(
            $scope, $http,
            "/api/TestData/" + $scope.qEmployeeNum + "/" + $scope.qSocId,
            "dailyTimeSheet_org", function () {

                $scope.dailyTimeSheet = $scope.dailyTimeSheet_org;
            }
        );
    }

    $scope.itemsByPage = 8;

    $scope.getColumns = function () {
        var ret = [];
        var arr = $scope.dailyTimeSheet;
        if (arr && arr.length > 0) {
            var row1 = arr[0];
            for (var key in row1) {
                //angular uses $$ to store internal values
                if (!key.startsWith("$$")) ret.push(key);
            }
        }

        if (ret.length == 0) ret.push("Row");
        return ret;
    }

    $scope.clearExpressions = function () {
        $scope.expressions = [];
    }

    $scope.getVariableInputStyle = function (item) {
        if (item && item.Variable) {
            if (item.Variable.startsWith("@") || item.Variable.startsWith("$$")) {
                return { backgroundColor: "#b1f9b4" }
            } else if (item.Variable.startsWith("$")) {
                return { backgroundColor: "#d0f3ea" }
            }
        }
        return {};
    }

    $scope.getResultMessageStyle = function () {
        if ($scope.syntaxOk)
            return { 'color': 'green' };
        else
            return { 'color': 'red' };
    }


    $scope.test = function () {

        $scope.bwsTimeSheet = { }

        $scope.calculate($scope.expressions, $scope.dailyTimeSheet_org, $scope.bwsTimeSheet);

    }

    $scope.executionResult = null;
    $scope.checkingSyntax = false;
    $scope.request = 0;

    $scope.calculate = function (expressions, dailyTimeSheet, bwsTimeSheet) {

        var url = '/api/Engine/Calculate/';
        var data = { 
            Expressions: expressions,
            Context: [
                { Data: dailyTimeSheet, Variables: bwsTimeSheet, DebugExecutionResult: true, Id: 'Request' + ++$scope.request }
            ]
        };
        var config = {};

        $scope.callData = data;

        if (!$scope.checkingSyntax) {

            $scope.checkingSyntax = true;

            $http.post(url, data, config)
                .success(function (data, status, headers) {
                    $scope.checkingSyntax = false;
                    if (data && data.Message) {
                        $scope.resultMessage = data.Message;
                        $scope.syntaxOk = data.Ok;
                        $scope.executionResult = { Ok: data.Ok, Message: data.Message, Result: data.Result, ExecutionTime: data.ExecutionTime };
                        if (data.Debug && data.Debug.length >= 1)
                            $scope.dailyTimeSheet = data.Debug[0].Data;
                    }
                })
                .error(function (error) {
                    $scope.checkingSyntax = false;
                    $scope.resultMessage = (error) ? error.toString() : "Erro";
                    $scope.syntaxOk = false;
                });

        }

    }

    $scope.callData = {};

    $scope.show_jsonImport = function () {
        $scope.jsonImport = angular.toJson($scope.expressions);
        $("#formulaImportJson").show();
    }

    $scope.cancel_jsonImport = function () {
        $scope.jsonImport = "";
        $("#formulaImportJson").hide();
    }

    $scope.import_jsonImport = function () {

        try {

            var obj = angular.fromJson($scope.jsonImport);
            var impExpressions = [];
            var ok = true;
            //check the data
            if (obj.constructor === Array) {
                for (var i = 0; i < obj.length; i++) {
                    if (obj[i].Variable && obj[i].Expression) {
                        impExpressions.push({ Variable: obj[i].Variable, Expression: obj[i].Expression, Order: i })
                    } else {
                        ok = false;
                        break;
                    }

                }
            }

            if (ok) {

                for (var i = 0; i < impExpressions.length; i++) {
                    $scope.expressions.push(impExpressions[i]);
                }
                
            } else
                window.alert("Json was not in the correct format");

        } catch (err) {
            window.alert(err.toString());
        }

        $scope.jsonImport = "";
        $("#formulaImportJson").hide();
    }

    $scope.downloadAssembly = function () {


        var expression = "";
        for (var i = 0; i < $scope.expressions.length; i++) {
            var line = $scope.expressions[i];
            if (line && line.Variable && line.Expression)
                expression += line.Variable + " = {" + line.Expression + "}\n";
        }


        $("#ExpressionFrmElement").val(expression);
        $("#DownloadForm").submit();
    }

});