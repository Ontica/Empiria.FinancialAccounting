/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification                           Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Use case interactor class               *
*  Type     : ReclassifiedServices                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services that build and return reclassified accounts.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

namespace Empiria.FinancialAccounting.Reclassification.Services {

  /// <summary>Provides services that build and return reclassified accounts.</summary>
  public class ReclassifiedServices : UseCase {

    #region Constructors and parsers

    protected ReclassifiedServices() {
      // no-op
    }

    static public ReclassifiedServices UseCaseInteractor() {
      return UseCase.CreateInstance<ReclassifiedServices>();
    }

    #endregion Constructors and parsers

    #region Use cases

    #endregion Use cases

    #region Helpers

    #endregion Helpers

  } // class ReclassifiedServices

} // namespace Empiria.FinancialAccounting.Reclassification.Services
