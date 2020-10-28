function QueryGraph(plantCode, equipmentModel, equipmentClass, manufacturer, callback) {
    plantCode = plantCode == null ? [] : plantCode;
    equipmentModel = equipmentModel == null ? [] : equipmentModel;
    equipmentClass = equipmentClass == null ? [] : equipmentClass;
    manufacturer = manufacturer == null ? [] : manufacturer;
    additiveLength = equipmentModel.length + manufacturer.length + equipmentClass.length;
    if (plantCode.length > 0 && additiveLength > 0) {
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