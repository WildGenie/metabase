#!/bin/bash

begin_chapter "Add optical data to LBNL database"

json_file_path=$(
  query \
    http://lbnl.gov:5020/graphql/ \
    addOpticalDataToLbnlDatabase.graphql \
    "{ \
      \"glazingComponentId\": \"$GLAZING_COMPONENT_ID\" \
    }" \
)

begin_section "Stored LBNL glazing optical data timestamp in variable ..."
begin_paragraph

read \
  LBNL_GLAZING_OPTICAL_DATA_TIMESTAMP \
  < <(echo $(
      cat $json_file_path \
      | jq .data[].opticalData.timestamp \
      | tr --delete '"'
    )
  )
echo_error "LBNL glazing optical data timestamp: \e[32m$LBNL_GLAZING_OPTICAL_DATA_TIMESTAMP\e[0m"

end_chapter
