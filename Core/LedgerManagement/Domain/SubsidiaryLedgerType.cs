/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria General Object                  *
*  Type     : SubsidiaryLedgerType                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes the type of a subsidiary ledger (e.g. activo, clientes, proveedores, etc).           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting {

  /// <summary>Describes the type of a subsidiary ledger (e.g. activo, clientes, proveedores, etc).</summary>
  public class SubsidiaryLedgerType : GeneralObject {

    private SubsidiaryLedgerType() {
      // Required by Empiria Framework.
    }

    static public SubsidiaryLedgerType Parse(int id) {
      return BaseObject.ParseId<SubsidiaryLedgerType>(id);
    }


    public static SubsidiaryLedgerType Pending {
      get {
        return SubsidiaryLedgerType.Parse(19);
      }
    }


    static public FixedList<SubsidiaryLedgerType> GetList() {
      return BaseObject.GetList<SubsidiaryLedgerType>()
                       .ToFixedList();
    }

    static public SubsidiaryLedgerType Empty => BaseObject.ParseEmpty<SubsidiaryLedgerType>();

  } // class SubsidiaryLedgerType

}  // namespace Empiria.FinancialAccounting
