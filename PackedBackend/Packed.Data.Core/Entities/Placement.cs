// Date Created: 2022/12/10
// Created by: JSW

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packed.Data.Core.Entities
{
    /// <summary>
    /// Class representing a row in the placement table
    /// </summary>
    [Table("placement")]
    public class Placement
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        [Column("placement_id")]
        public int Id { get; set; }

        [ForeignKey(nameof(Item))]
        [Column("item_id")]
        public int ItemId { get; set; }

        /// <summary>
        /// Navigation property to the item this placement is for
        /// </summary>
        public Item Item { get; set; }

        [ForeignKey(nameof(Container))]
        [Column("container_id")]
        public int ContainerId { get; set; }

        /// <summary>
        /// Navigation property to the container the item was placed into
        /// </summary>
        public Container Container { get; set; }
    }
}