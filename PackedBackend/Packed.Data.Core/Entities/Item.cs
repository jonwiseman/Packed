// Date Created: 2022/12/10
// Created by: JSW

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packed.Data.Core.Entities
{
    /// <summary>
    /// Class representing a row in the item table
    /// </summary>
    [Table("item")]
    public class Item
    {
        /// <summary>
        /// Item ID
        /// </summary>
        [Key]
        [Column("item_id")]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the list that this
        /// item belongs to
        /// </summary>
        [ForeignKey(nameof(List))]
        [Column("list_id")]
        public int ListId { get; set; }

        /// <summary>
        /// Navigation property to the list this item belongs to
        /// </summary>
        public List List { get; set; }

        /// <summary>
        /// Name of the item
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Total count of this item in the list
        /// </summary>
        [Column("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Navigation collection to all placements for this item
        /// </summary>
        public List<Placement> Placements { get; set; }
    }
}