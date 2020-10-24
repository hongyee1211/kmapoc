function QueryGraph(plantCode, equipmentModel, equipmentClass, manufacturer, callback) {
    plantCode = plantCode == null ? [] : plantCode;
    equipmentModel = equipmentModel == null ? [] : equipmentModel;
    equipmentClass = equipmentClass == null ? [] : equipmentClass;
    manufacturer = manufacturer == null ? [] : manufacturer;
    additiveLength = plantCode.length + equipmentModel.length + manufacturer.length + equipmentClass.length;
    if (additiveLength > 1) {
        $.post('/api/graph/query',
            {
                plantCode: plantCode,
                equipmentModel: equipmentModel,
                equipmentClass: equipmentClass,
                manufacturer: manufacturer,
            },
            function (data) {
                callback(data)
            }
        )
    }
}