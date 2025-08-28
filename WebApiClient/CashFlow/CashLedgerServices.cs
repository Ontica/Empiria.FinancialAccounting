/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Data Layer                              *
*  Assembly : FinancialAccounting.WebApiClient.dll       Pattern   : Web api client                          *
*  Type     : CashLedgerServices                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides financial accounting cash ledger transactions services using a web proxy.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Threading.Tasks;

namespace Empiria.FinancialAccounting.ClientServices {

  /// <summary>Provides financial accounting cash ledger transactions services using a web proxy.</summary>
  public class CashLedgerServices : BaseService {

    public async Task<FixedList<NamedEntityDto>> GetAccountingLedgers() {

      string path = "v2/financial-accounting/ledgers/ifrs";

      return await WebApiClient.GetAsync<FixedList<NamedEntityDto>>(path);
    }


    public async Task<FixedList<NamedEntityDto>> GetTransactionSources() {

      string path = "v2/financial-accounting/vouchers/functional-areas";

      return await WebApiClient.GetAsync<FixedList<NamedEntityDto>>(path);
    }


    public async Task<FixedList<NamedEntityDto>> GetTransactionTypes() {

      string path = "v2/financial-accounting/vouchers/transaction-types";

      return await WebApiClient.GetAsync<FixedList<NamedEntityDto>>(path);
    }


    public async Task<FixedList<NamedEntityDto>> GetVoucherTypes() {

      string path = "v2/financial-accounting/vouchers/voucher-types";

      return await WebApiClient.GetAsync<FixedList<NamedEntityDto>>(path);
    }

  }  // class CashLedgerServices

}  // namespace Empiria.FinancialAccounting.ClientServices
