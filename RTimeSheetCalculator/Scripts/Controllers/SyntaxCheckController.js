//'TimeSheetApp.HttpInterceptor'

var app = angular.module('TimeSheetApp', ['smart-table'], function ($locationProvider) {
    $locationProvider.html5Mode({
        enabled: false,
        requireBase: false
    });
}, null);

app.controller('PageController', function ($scope, $http, $location, $timeout) {

    $scope.expression = "";
    $scope.syntaxOk = true;
    $scope.resultMessage = "";
    $scope.executeExpression = true;
    $scope.executionResult = null;
    $scope.expressionLines = [];
    $scope.dataTable = [];
    $scope.displayDataTable = [];

    $scope.itemsByPage = 8;

    window._helper.app.angular.buildRepeatingAreaFunctions(
        "Line", $scope,
        function () {
            if (!$scope.expressionLines) $scope.expressionLines = [];
            return $scope.expressionLines;
        }, function () {
            var index = $scope.expressionLines.length - 1;
            $timeout(function () { $("#var_" + index).focus(); }, 0, false);
        }
    );

    $scope.getColumns = function () {
        var ret = [];
        var arr = $scope.dataTable;
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
        $scope.expressionLines = [];
        $scope.dataTable = [];
    }

    $scope.getVariableInputStyle = function (item) {
        if (item && item.var) {
            if (item.var.startsWith("@") || item.var.startsWith("$$")) {
                return { backgroundColor: "#b1f9b4" }
            } else if (item.var.startsWith("$")) {
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

    $scope.$watch("expressionLines", function () {

        $scope.expression = "";
        for (var i = 0; i < $scope.expressionLines.length; i++) {
            var line = $scope.expressionLines[i];
            if (line && line.var && line.expression)
                $scope.expression += line.var + " = {" + line.expression + "}\n";
        }

        if ($scope.expression)
            $scope.checkSyntax($scope.expression, $scope.executeExpression);

    }, true);

    $scope.apply = function () {
        if ($scope.expression)
            $scope.checkSyntax($scope.expression, $scope.executeExpression);
    };

    $scope.checkingSyntax = false;
    $scope.needsCheck = false;
    $scope.checkSyntax = function (expression, execute) {

        //var url = '/api/Syntax/Exec/';
        var url = 'https://h613bd9vq9.execute-api.eu-west-1.amazonaws.com/dev'
        var data = { Expression: expression, Execute: execute };
        var config = {};

        if (!$scope.checkingSyntax) {

            $scope.checkingSyntax = true;
            $scope.needsCheck = false;

            $http.post(url, data, config)
                .success(function (data, status, headers) {
                    $scope.checkingSyntax = false;
                    if (data && data.Message) {
                        $scope.resultMessage = data.Message;
                        $scope.syntaxOk = data.Ok;
                        $scope.executionResult = { Ok: data.Ok, Message: data.Message, Result: data.Result.ExecutionContext.Values };
                        if (data.Result && data.Result.ExecutionContext)
                            $scope.dataTable = data.Result.ExecutionContext.Data;
                    }

                    if ($scope.needsCheck) {
                        $timeout($scope.apply);
                    }

                })
                .error(function (error) {
                    $scope.checkingSyntax = false;
                    $scope.resultMessage = (error) ? error.toString() : "Erro";
                    $scope.syntaxOk = false;

                    if ($scope.needsCheck) {
                        $timeout($scope.apply);
                    }

                });

        } else
            $scope.needsCheck = true;

    }

    $scope.downloadAssembly = function () {
        $("#ExpressionFrmElement").val($scope.expression);
        $("#DownloadForm").submit();
    }

});