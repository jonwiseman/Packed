// Date Created: 2022/12/10
// Created by: JSW

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packed.Data.Core.Entities
{
    /// <summary>
    /// Class representing a row in the list table
    /// </summary>
    [Table("list")]
    public class List
    {
        /// <summary>
        /// Identifier for the list
        /// </summary>
        [Key]
        [Column("list_id")]
        public int Id { get; set; }

        /// <summary>
        /// List description
        /// </summary>
        [Column("description")]
        public string Description { get; set; }

        /// <summary>
        /// Navigation collection representing all items associated with this list
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// Navigation collection representing all containers associated with this list
        /// </summary>
        public List<Container> Containers { get; set; }
    }
}