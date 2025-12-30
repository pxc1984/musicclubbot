-- Add thumbnail_url field to song table
ALTER TABLE song ADD COLUMN IF NOT EXISTS thumbnail_url TEXT;
