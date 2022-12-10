// Date Created: 2022/12/10
// Created by: JSW

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packed.Data.Core.Entities
{
    /// <summary>
    /// Class representing a row in the container table
    /// </summary>
    [Table("container")]
    public class Container
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        [Column("container_id")]
        public int Id { get; set; }

        /// <summary>
        /// ID of the list that this container belongs to
        /// </summary>
        [ForeignKey(nameof(List))]
        [Column("list_id")]
        public int ListId { get; set; }

        /// <summary>
        /// Navigation property to the list this container belongs to
        /// </summary>
        public List List { get; set; }

        /// <summary>
        /// Name of the container
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Navigation collection to all placements for this container
        /// </summary>
        public List<Placement> Placements { get; set; }
    }
}