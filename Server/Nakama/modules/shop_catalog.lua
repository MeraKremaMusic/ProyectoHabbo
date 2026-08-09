local catalog = {}


local PRODUCTS = {

    {
        id = "cubo_2x2",
        name = "Cubo 2x2",
        category = "muebles",
        price = 250
    },

    {
        id = "silla_1x1",
        name = "Silla",
        category = "muebles",
        price = 150
    },

    {
        id = "planta_1x1",
        name = "Planta",
        category = "muebles",
        price = 200
    },
    {
    id = "lampara_1x1",
    name = "Lampara de pie",
    category = "muebles",
    price = 200
}

}


function catalog.get_all()

    return PRODUCTS

end


function catalog.find(
    product_id
)

    if
        product_id == nil
        or
        product_id == ""
    then
        return nil
    end


    for _, product
        in ipairs(PRODUCTS)
    do

        if product.id == product_id then

            return product

        end

    end


    return nil

end


return catalog