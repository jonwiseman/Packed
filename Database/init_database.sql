-- Create the list table. Descriptions must be unique
CREATE TABLE IF NOT EXISTS list
(
    list_id     SERIAL PRIMARY KEY,
    description TEXT NOT NULL UNIQUE
);

-- Create the table representing items. Note that the quantity of items must be positive and each item
-- must have a unique name within a list
CREATE TABLE IF NOT EXISTS item
(
    item_id  SERIAL PRIMARY KEY,
    list_id  INT REFERENCES list,
    name     TEXT NOT NULL,
    quantity INT  NOT NULL DEFAULT (1) CHECK (quantity > 0),
    UNIQUE (list_id, name)
);

-- Create the container table. Each container must have a unique name within a list
CREATE TABLE IF NOT EXISTS container
(
    container_id SERIAL PRIMARY KEY,
    list_id      INT REFERENCES list,
    name         TEXT NOT NULL,
    UNIQUE (list_id, name)
);

-- Create the placement table. Each row represents a the placement of an item into a container. For example,
-- if we had two items named "item a" and wanted to place them both into "container a," then we'd end up with
-- two rows in this table
CREATE TABLE IF NOT EXISTS placement
(
    placement_id SERIAL PRIMARY KEY,
    item_id      INT REFERENCES item,
    container_id INT REFERENCES container
);