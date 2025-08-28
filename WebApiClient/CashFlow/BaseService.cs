/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Data Layer                              *
*  Assembly : FinancialAccounting.WebApiClient.dll       Pattern   : Abstract type                           *
*  Type     : BaseService                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Base abstract type used to inherit all services.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.WebApi.Client;

namespace Empiria.FinancialAccounting.ClientServices {

  /// <summary>Base abstract type used to inherit all services.</summary>
  abstract public class BaseService {

    public BaseService() {
      WebApiClient = WebApiClient.GetInstance("SICOFIN");
    }

    protected WebApiClient WebApiClient {
      get;
    }

  }  // class BaseService

}  // namespace Empiria.FinancialAccounting.ClientServices
