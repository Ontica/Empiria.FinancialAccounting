/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : AccountingOperation                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Used to read accounting operation rules.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.Reclassification.Data;

namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>Used to read accounting operation rules.</summary>
  public class AccountingOperation {

    #region Constructors and Parsers

    internal AccountingOperation() {
      // Required by Empiria Framework
    }

    #endregion Constructors and Parsers

    #region Properties

    [DataField("ID_REGLA")]
    public int Id {
      get; set;
    }


    [DataField("UID_REGLA")]
    public string UID {
      get; set;
    }


    [DataField("ID_TIPO_OPERACION")]
    public AccountingOperationType AccountingOperationType {
      get; set;
    }


    [DataField("CUENTA_DEBE")]
    public string DebitAccount {
      get; set;
    }


    [DataField("NOMBRE_CUENTA_DEBE")]
    public string DebitAccountName {
      get; set;
    }


    [DataField("CUENTA_HABER")]
    public string CreditAccount {
      get; set;
    }


    [DataField("NOMBRE_CUENTA_HABER")]
    public string CreditAccountName {
      get; set;
    }


    [DataField("STATUS", Default = 'A')]
    public char Status {
      get; set;
    }

    #endregion Properties

    #region Methods

    static public FixedList<AccountingOperation> GetList() {
      return AccountingOperationDataService.GetOperations();
    }


    #endregion Methods


  } // class AccountingOperation

} // namespace Empiria.FinancialAccounting.Reclassification
