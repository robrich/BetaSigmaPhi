namespace BetaSigmaPhi.Entity {
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class Document : IEntity {

		public const int SLUG_MAX_LENGTH = 80;
		public const int ROOT_NODE_ID = 1;
		public const char NODE_PATH_SEPARATOR = '/';

		public Document() {
			this.Depth = -1; // Not set yet
		}

		[Key]
		public int DocumentId {
			get { return this.Id; }
			set { this.Id = value; }
		}

		[Column( "NodeTypeId" )]
		public NodeType NodeType { get; set; }

		public int ParentDocumentId { get; set; }
		[ForeignKey("ParentDocumentId")]
		public Document ParentDocument { get; set; }

		/// <summary>
		/// /Document.ROOT_NODE_ID/node/ids/of/path/to/and/including/this/NodeId
		/// </summary>
		[Required]
		[StringLength( 400 )]
		public string NodeIdPath { get; set; }

		/// <summary>
		/// Portion of the url for this node, Document.ROOT_NODE_ID is blank, all others aren't
		/// </summary>
		[Required]
		[StringLength( 80 )]
		public string Slug { get; set; }

		[Required]
		[StringLength( 80 )]
		public string Name { get; set; }

		// nvarchar(max)
		[DataType( DataType.MultilineText )]
		public string PageContent { get; set; }

		// nvarchar(max)
		[DataType( DataType.MultilineText )]
		public string HeadContent { get; set; }

		// nvarchar(max)
		[DataType( DataType.MultilineText )]
		public string ScriptContent { get; set; }

		// Convinience properties:
		public int Depth { get; set; }

		/* If needed:
		public int Sequence { get; set; }

		public DateTime? PublishDate { get; set; }

		[StringLength( 500 )]
		public string Description { get; set; }

		[StringLength( 500 )]
		public string Keywords { get; set; }
		*/

	}
}
