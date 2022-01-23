/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria General Object                  *
*  Type     : TransactionType                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a business transaction type related with an accounting voucher.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Describes a business transaction type related with an accounting voucher.</summary>
  public class TransactionType : GeneralObject {

    private TransactionType() {
      // Required by Empiria Framework.
    }

    static public TransactionType Parse(int id) {
      return BaseObject.ParseId<TransactionType>(id);
    }


    static public TransactionType Parse(string uid) {
      return BaseObject.ParseKey<TransactionType>(uid);
    }


    static public FixedList<TransactionType> GetList() {
      return BaseObject.GetList<TransactionType>(string.Empty, "ObjectName")
                       .ToFixedList();
    }

    static public TransactionType Empty => BaseObject.ParseEmpty<TransactionType>();


    static public TransactionType Automatic {
      get {
        return TransactionType.Parse("58892e04-b66c-47f7-8766-799663ea776b");
      }
    }


    static public TransactionType Manual {
      get {
        return TransactionType.Parse("6ea907ee-1534-49ba-9678-a13e90fdf6d2");
      }
    }


  } // class TransactionType

}  // namespace Empiria.FinancialAccounting.Vouchers
