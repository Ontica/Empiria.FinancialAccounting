/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria Data Object                     *
*  Type     : EventType                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data about a functional area.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers.Data;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Holds data about a functional area.</summary>
  public class EventType : BaseObject {

    #region Constructors and parsers

    protected EventType() {
      // Required by Empiria Framework.
    }

    static public EventType Parse(int id) {
      return BaseObject.ParseId<EventType>(id);
    }

    static public FixedList<EventType> GetList() {
      return VoucherData.EventTypes();
    }

    static public EventType Empty {
      get {
        return BaseObject.ParseEmpty<EventType>(0);
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("DESCRICPION_EVENTO")]
    public string Name {
      get; private set;
    }

    #endregion Properties

    public NamedEntityDto MapToNamedEntity() {
      return new NamedEntityDto(this.Id.ToString(), this.Name);
    }

  } // class EventType

}  // namespace Empiria.FinancialAccounting.Vouchers
