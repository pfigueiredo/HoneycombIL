if (!window._helper) window._helper = {};

window._helper.app = {};
window._helper.app.angular = {};

window._helper.app.angular.buildRepeatingAreaFunctions = function buildRepeatingAreaHelperFunctions(dataType, $scope, dataFunction, addCallback) {

    var moveUpFunctionName = "move" + dataType + "Up";
    var moveDownFunctionName = "move" + dataType + "Down";;
    var deleteFunctionName = "delete" + dataType;
    var addFunctionName = "add" + dataType;

    var orderFnc = function (data) {
        if (data && data.length) {
            for (var i = 0; i < data.length; i++) {
                data[i].Order = i;
            }
        }
    }

    var normalizeArray = function (arr) {
        arr.sort(function (a, b) { return a.Order - b.Order; });
        var order = 0;
        for (var i = 0; i < arr.length; i++)
            if (arr[i].Order >= 0) //Do not normalize deleted items;
                arr[i].Order = order++;

        return order;
    }

    var swapFnc = function (arr, item, relDest) {

        if (!('Order' in item)) orderFnc(arr);
        normalizeArray(arr);

        var fromOrder = item.Order;
        var toOrder = fromOrder + relDest;
        if (toOrder < 0) return; //don't go into deleted items space

        for (var i = 0; i < arr.length; i++) {
            if (arr[i].Order >= 0) { //Skip deleted items
                if (arr[i].Order === toOrder) {
                    arr[i].Order = fromOrder;
                    item.Order = toOrder;
                    break;
                }
            }
        }

        normalizeArray(arr);
    }

    $scope[moveUpFunctionName] = (function (fnc, swapFnc) {
        return function (item, data) {
            if (!data && fnc) data = fnc(); if (data) { swapFnc(data, item, -1); }
        };
    })(dataFunction, swapFnc);

    $scope[moveDownFunctionName] = (function (fnc, swapFnc) {
        return function (item, data) {
            if (!data && fnc) data = fnc(); if (data) { swapFnc(data, item, 1); }
        };
    })(dataFunction, swapFnc);

    $scope[deleteFunctionName] = (function (fnc, normalizeArray) {
        return function (item, data) {
            if (!data && fnc) data = fnc();
            if (data) {
                data.splice(data.indexOf(item), 1);
                normalizeArray(data);
            }
        };
    })(dataFunction, normalizeArray);

    $scope[addFunctionName] = (function (fnc, normalizeArray, callback) {
        return function (data) {
            if (!data && fnc) data = fnc();
            if (data) {
                var obj = {};
                obj.Order = normalizeArray(data);
                data.push(obj); orderFnc(data);
                if (callback) callback(obj, data);
                return obj;
            }
        };
    })(dataFunction, normalizeArray, addCallback);

}