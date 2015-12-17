namespace Example

open System
open System.Runtime.Serialization

module Outputs =

    [<CLIMutable>]
    [<DataContract>]
    type Product =
        {
            [<field: DataMember(Name = "Descr")>]
            Descr: string
            [<field: DataMember(Name = "Price")>]
            Price: decimal
        }

    [<CLIMutable>]
    [<DataContract>]
    type Person =
        {
            [<field: DataMember(Name = "Id")>]
            Id: int
            [<field: DataMember(Name = "Name")>]
            Name: string
            [<field: DataMember(Name = "Age")>]
            Age: int
            [<field: DataMember(Name = "Products")>]
            Products: Product list
        }
