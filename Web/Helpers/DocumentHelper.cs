namespace BetaSigmaPhi.Web.Helpers {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Repository;

	public static class DocumentHelper {
		private static readonly IDocumentRepository documentRepository = ServiceLocator.GetService<IDocumentRepository>();

		public static List<Document> DocumentRootMenu(this HtmlHelper HtmlHelper) {
			return documentRepository.GetImediateChildren(Document.ROOT_NODE_ID, ActiveOnly: true);
		}

	}
}
