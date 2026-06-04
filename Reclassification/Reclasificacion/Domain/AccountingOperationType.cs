/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclasfication                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria General Object                  *
*  Type     : AccountingOperationType                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an accouting operation type.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>Represents an accouting operation type.</summary>
  public class AccountingOperationType : GeneralObject {

    #region Constructors and parsers

    protected AccountingOperationType() {
      // Required by Empiria Framework.
    }

    static public AccountingOperationType Parse(int id) => ParseId<AccountingOperationType>(id);

    static public AccountingOperationType Parse(string uid) => ParseKey<AccountingOperationType>(uid);

    static public FixedList<AccountingOperationType> GetList() {
      return BaseObject.GetList<AccountingOperationType>()
                       .ToFixedList();
    }

    static public AccountingOperationType Empty => ParseEmpty<AccountingOperationType>();

    #endregion Constructors and parsers

    #region Properties

    public new string Name {
      get {
        if (IsEmptyInstance) {
          return "Otras transacciones";
        }
        return base.Name;
      }
    }

    #endregion Properties

  } // class AccountingOperationType

}  // namespace Empiria.FinancialAccounting.Reclassification
